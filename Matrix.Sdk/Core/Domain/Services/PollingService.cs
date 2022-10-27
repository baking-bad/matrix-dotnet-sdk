namespace Matrix.Sdk.Core.Domain.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Infrastructure.Dto.Sync;
    using Infrastructure.Services;
    using MatrixRoom;
    using Microsoft.Extensions.Logging;

    public class PollingService : IPollingService
    {
        private readonly EventService _eventService;
        private readonly ILogger<PollingService>? _logger;

        private ConcurrentDictionary<string, MatrixRoom> _matrixRooms;
        private CancellationTokenSource _cts;
        private string? _accessToken;
        private string _nextBatch;
        private Timer? _pollingTimer;
        private ulong _timeout;

        public PollingService(EventService eventService, ILogger<PollingService>? logger)
        {
            _eventService = eventService;
            _logger = logger;
            _timeout = Constants.FirstSyncTimout;
        }

        public bool IsSyncing { get; private set; }

        public event EventHandler<SyncBatchEventArgs> OnSyncBatchReceived;

        public MatrixRoom[] InvitedRooms =>
            _matrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Invited).ToArray();

        public MatrixRoom[] JoinedRooms =>
            _matrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Joined).ToArray();

        public MatrixRoom[] LeftRooms => _matrixRooms.Values.Where(x => x.Status == MatrixRoomStatus.Left).ToArray();

        public void Init(Uri nodeAddress, string accessToken)
        {
            _eventService.BaseAddress = nodeAddress;
            _accessToken = accessToken;
            _cts = new CancellationTokenSource();
            _matrixRooms = new ConcurrentDictionary<string, MatrixRoom>();
            _pollingTimer = new Timer(async _ => await PollAsync());
        }

        public void Start(string? nextBatch = null)
        {
            if (_pollingTimer == null)
                throw new NullReferenceException("Call Init first.");

            if (nextBatch != null)
                _nextBatch = nextBatch;

            _pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
            IsSyncing = true;
        }

        public void Stop()
        {
            _cts.Cancel();
            _pollingTimer!.Change(Timeout.Infinite, Timeout.Infinite);
            IsSyncing = false;
        }

        public MatrixRoom? GetMatrixRoom(string roomId) =>
            _matrixRooms.TryGetValue(roomId, out MatrixRoom matrixRoom) ? matrixRoom : null;

        public void Dispose()
        {
            _cts.Dispose();
            _pollingTimer?.Dispose();
        }

        private async Task PollAsync()
        {
            try
            {
                _pollingTimer!.Change(Timeout.Infinite, Timeout.Infinite);
                IsSyncing = true;

                SyncResponse response = await _eventService.SyncAsync(_accessToken!, _cts.Token,
                    _timeout, _nextBatch);
                SyncBatch syncBatch = SyncBatch.Factory.CreateFromSync(response.NextBatch, response.Rooms);

                _nextBatch = syncBatch.NextBatch;
                _timeout = Constants.LaterSyncTimout;

                RefreshRooms(syncBatch.MatrixRooms);
                OnSyncBatchReceived.Invoke(this, new SyncBatchEventArgs(syncBatch));

                // immediately call timer cb (this method)
                _pollingTimer?.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
            }
            catch (TaskCanceledException ex)
            {
                if (!_cts.IsCancellationRequested)
                {
                    _pollingTimer?
                        .Change(TimeSpan.FromMilliseconds(Constants.LaterSyncTimout), TimeSpan.FromMilliseconds(-1));
                }

                IsSyncing = false;
                _logger?.LogError(
                    "Polling cancelled, _cts.IsCancellationRequested {@IsCancellationRequested}:, {@Message}",
                    _cts.IsCancellationRequested, ex.Message);
            }
            catch (Exception ex)
            {
                _pollingTimer?
                    .Change(TimeSpan.FromMilliseconds(Constants.LaterSyncTimout), TimeSpan.FromMilliseconds(-1));

                IsSyncing = false;
                _logger?.LogError("Polling: exception occured. Message: {@Message}", ex.Message);
            }
        }

        private void RefreshRooms(List<MatrixRoom> matrixRooms)
        {
            foreach (MatrixRoom room in matrixRooms)
                if (!_matrixRooms.TryGetValue(room.Id, out MatrixRoom retrievedRoom))
                {
                    if (!_matrixRooms.TryAdd(room.Id, room))
                        _logger?.LogError("Can not add matrix room");
                }
                else
                {
                    var updatedUserIds = retrievedRoom
                        .JoinedUserIds
                        .Concat(room.JoinedUserIds)
                        .Distinct()
                        .ToList();

                    var updatedRoom = new MatrixRoom(retrievedRoom.Id, room.Status, updatedUserIds);

                    _matrixRooms.TryUpdate(room.Id, updatedRoom, retrievedRoom);
                }
        }
    }
}