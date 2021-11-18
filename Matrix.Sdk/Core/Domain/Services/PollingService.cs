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
        private readonly CancellationTokenSource _cts = new();
        private readonly EventService _eventService;
        private readonly ILogger<PollingService>? _logger;
        private readonly ConcurrentDictionary<string, MatrixRoom> _matrixRooms = new();

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

            _pollingTimer = new Timer(async _ => await PollAsync());
        }

        public void Start()
        {
            if (_pollingTimer == null)
                throw new NullReferenceException("Call Init first.");

            _pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
        }

        public void Stop()
        {
            _cts.Cancel();
            _pollingTimer!.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void UpdateMatrixRoom(string roomId, MatrixRoom matrixRoom)
        {
            if (!_matrixRooms.TryGetValue(roomId, out MatrixRoom oldValue))
                _logger?.LogInformation($"RoomId: {roomId}: could not get value");

            if (!_matrixRooms.TryUpdate(roomId, matrixRoom, oldValue))
                _logger?.LogInformation($"RoomId: {roomId}: could not update value");
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

                SyncResponse response = await _eventService.SyncAsync(_accessToken!, _cts.Token,
                    _timeout, _nextBatch);

                SyncBatch syncBatch = SyncBatch.Factory.CreateFromSync(response.NextBatch, response.Rooms);

                _nextBatch = syncBatch.NextBatch;
                _timeout = Constants.LaterSyncTimout;

                RefreshRooms(syncBatch.MatrixRooms);
                OnSyncBatchReceived.Invoke(this, new SyncBatchEventArgs(syncBatch));

                _pollingTimer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(-1));
            }
            catch (TaskCanceledException)
            {
                _logger?.LogInformation("Polling: HTTP Get request canceled");
            }
            catch (Exception)
            {
                _logger?.LogError("Exception");
            }
        }

        private void RefreshRooms(List<MatrixRoom> matrixRooms)
        {
            foreach (MatrixRoom room in matrixRooms)
                if (!_matrixRooms.TryGetValue(room.Id, out MatrixRoom retrievedRoom))
                {
                    _matrixRooms.TryAdd(room.Id, room);
                }
                else
                {
                    var updatedUserIds = retrievedRoom.JoinedUserIds.Concat(room.JoinedUserIds).ToList();
                    var updatedRoom = new MatrixRoom(retrievedRoom.Id, room.Status, updatedUserIds);

                    _matrixRooms.TryUpdate(room.Id, updatedRoom, retrievedRoom);
                }
        }
    }
}