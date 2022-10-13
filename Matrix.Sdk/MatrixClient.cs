namespace Matrix.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Domain;
    using Core.Domain.MatrixRoom;
    using Core.Domain.Services;
    using Core.Infrastructure.Dto.Event;
    using Core.Infrastructure.Dto.Login;
    using Core.Infrastructure.Dto.Room.Create;
    using Core.Infrastructure.Dto.Room.Join;
    using Core.Infrastructure.Dto.Room.Joined;
    using Core.Infrastructure.Services;

    /// <summary>
    ///     A Client for interaction with Matrix.
    /// </summary>
    public class MatrixClient : IMatrixClient
    {
        private readonly CancellationTokenSource _cts = new();
        private readonly IPollingService _pollingService;
        
        private readonly UserService _userService;
        private readonly RoomService _roomService;
        private readonly EventService _eventService;
        
       
        private string? _accessToken;
        private ulong _transactionNumber;

        public MatrixClient(
            IPollingService pollingService,
            UserService userService, 
            RoomService roomService, 
            EventService eventService)
        {
            _pollingService = pollingService;
            _userService = userService;
            _roomService = roomService;
            _eventService = eventService;
        }

        public event EventHandler<MatrixRoomEventsEventArgs> OnMatrixRoomEventsReceived;

        public string UserId { get; private set; }

        public Uri? BaseAddress { get; private set; }

        public bool IsLoggedIn { get; private set; }
        
        public bool IsSyncing { get; private set; }

        public MatrixRoom[] InvitedRooms => _pollingService.InvitedRooms;
        
        public MatrixRoom[] JoinedRooms => _pollingService.JoinedRooms;

        public MatrixRoom[] LeftRooms => _pollingService.LeftRooms;

        public async Task LoginAsync(Uri baseAddress, string user, string password, string deviceId)
        {
            _userService.BaseAddress = baseAddress;
            _roomService.BaseAddress = baseAddress;
            _eventService.BaseAddress = baseAddress;
            BaseAddress = baseAddress;
            
            LoginResponse response = await _userService.LoginAsync(user, password, deviceId, _cts.Token);

            UserId = response.UserId;
            _accessToken = response.AccessToken;

            _pollingService.Init(baseAddress, _accessToken);

            IsLoggedIn = true;
        }

        public void Start(string? nextBatch = null)
        {
            if (!IsLoggedIn)
                throw new Exception("Call LoginAsync first");

            _pollingService.OnSyncBatchReceived += OnSyncBatchReceived;
            _pollingService.Start(nextBatch);

            IsSyncing = _pollingService.IsSyncing;
        }

        public void Stop()
        {
            _pollingService.Stop();
            _pollingService.OnSyncBatchReceived -= OnSyncBatchReceived;

            IsSyncing = _pollingService.IsSyncing;
        }

        public async Task<CreateRoomResponse> CreateTrustedPrivateRoomAsync(string[] invitedUserIds) =>
            await _roomService.CreateRoomAsync(_accessToken!, invitedUserIds, _cts.Token);

        public async Task<JoinRoomResponse> JoinTrustedPrivateRoomAsync(string roomId)
        {
            MatrixRoom? matrixRoom = _pollingService.GetMatrixRoom(roomId);
            if (matrixRoom != null && matrixRoom.Status != MatrixRoomStatus.Invited)
                return new JoinRoomResponse(matrixRoom.Id);

            return await _roomService.JoinRoomAsync(_accessToken!, roomId, _cts.Token);
        }

        public async Task<string> SendMessageAsync(string roomId, string message)
        {
            string transactionId = CreateTransactionId();
            
            EventResponse eventResponse = await _eventService.SendMessageAsync(_accessToken!,
                roomId, transactionId, message, _cts.Token);

            if (eventResponse.EventId == null)
                throw new NullReferenceException(nameof(eventResponse.EventId));

            return eventResponse.EventId;
            
        }

        public async Task<List<string>> GetJoinedRoomsIdsAsync()
        {
            JoinedRoomsResponse response =
                await _roomService.GetJoinedRoomsAsync(_accessToken!, _cts.Token);

            return response.JoinedRoomIds;
        }

        public async Task LeaveRoomAsync(string roomId) => 
            await _roomService.LeaveRoomAsync(_accessToken!, roomId, _cts.Token);

        private void OnSyncBatchReceived(object? sender, SyncBatchEventArgs syncBatchEventArgs)
        {
            if (sender is not IPollingService)
                throw new ArgumentException("sender is not polling service");

            SyncBatch batch = syncBatchEventArgs.SyncBatch;

            OnMatrixRoomEventsReceived.Invoke(this, new MatrixRoomEventsEventArgs(batch.MatrixRoomEvents,  batch.NextBatch));
        }

        private string CreateTransactionId()
        {
            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            ulong counter = _transactionNumber;

            _transactionNumber += 1;

            return $"m{timestamp}.{counter}";
        }
    }
}