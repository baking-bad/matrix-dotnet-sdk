using System.Runtime.CompilerServices.Dto.User;
using Matrix.Sdk.Core.Domain.RoomEvent;

namespace Matrix.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Core.Domain;
    using Core.Domain.Room;
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
        private readonly MediaService _mediaService;
        
       
        private string? _accessToken;
        private ulong _transactionNumber;

        public MatrixClient(
            IPollingService pollingService,
            UserService userService, 
            RoomService roomService, 
            EventService eventService,
            MediaService mediaService)
        {
            _pollingService = pollingService;
            _userService = userService;
            _roomService = roomService;
            _eventService = eventService;
            _mediaService = mediaService;
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
            _mediaService.BaseAddress = baseAddress;
            BaseAddress = baseAddress;
            
            LoginResponse response = await _userService.LoginAsync(user, password, deviceId, _cts.Token);

            UserId = response.UserId;
            _accessToken = response.AccessToken;

            _pollingService.Init(baseAddress, _accessToken);

            IsLoggedIn = true;
        }

        public async Task LoginAsync(Uri baseAddress, string token, string? userId = null)
        {
            _userService.BaseAddress = baseAddress;
            _roomService.BaseAddress = baseAddress;
            _eventService.BaseAddress = baseAddress;
            _mediaService.BaseAddress = baseAddress;
            BaseAddress = baseAddress;

            UserId = userId;
            
            _accessToken = token;

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
        
        public async Task<string> SendImageAsync(string roomId, string filename, byte[] imageData)
        {
            string transactionId = CreateTransactionId();
            
            var mxcUrl = await _mediaService.UploadImage(_accessToken!, filename, imageData, _cts.Token);
            
            EventResponse eventResponse = await _eventService.SendImageAsync(_accessToken!,
                roomId, transactionId, filename, mxcUrl, _cts.Token);

            if (eventResponse.EventId == null)
                throw new NullReferenceException(nameof(eventResponse.EventId));

            return eventResponse.EventId;
        }

        public async Task<string> SendFileAsync(string roomId, string filename, byte[] blob)
        {
            string transactionId = CreateTransactionId();

            var mxcUrl = await _mediaService.UploadFile(_accessToken!, filename, blob, _cts.Token);

            EventResponse eventResponse = await _eventService.SendFileAsync(_accessToken!,
                roomId, transactionId, filename, mxcUrl, _cts.Token);

            if (eventResponse.EventId == null)
                throw new NullReferenceException(nameof(eventResponse.EventId));

            return eventResponse.EventId;
        }

        public async Task<string> GetString(string url)
        {
            return await _eventService.GetString(_accessToken!, url, _cts.Token);
        }

        public async Task<List<string>> GetJoinedRoomsIdsAsync()
        {
            return await _roomService.GetJoinedRoomsAsync(_accessToken!, _cts.Token);
        }

        public async Task LeaveRoomAsync(string roomId) => 
            await _roomService.LeaveRoomAsync(_accessToken!, roomId, _cts.Token);

        public async Task<List<BaseRoomEvent>> GetHistory(string roomId,
            Func<BaseRoomEvent, Task<bool>> stopCallback)
        {
            return await GetHistory(roomId, null, stopCallback);
        }
        
        public async Task<List<BaseRoomEvent>> GetHistory(string roomId, string fromEventId, Func<BaseRoomEvent, Task<bool>> stopCallback)
        {
            return await _eventService.GetTimelineEventsAsync(_accessToken!, roomId, null, stopCallback, _cts.Token);
        }

        public async Task<string> EditMessage(string roomId, string messageId, string newText)
        {
            string transactionId = CreateTransactionId();
            
            EventResponse eventResponse = await _eventService.EditMessageAsync(_accessToken!,
                roomId, transactionId, messageId, newText, _cts.Token);

            if (eventResponse.EventId == null)
                throw new NullReferenceException(nameof(eventResponse.EventId));

            return eventResponse.EventId;
        }

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

        public async Task SendTypingSignal(string roomId, TimeSpan timeout)
        {
            await _eventService.SendTypingSignalAsync(_accessToken!, roomId, UserId, timeout, _cts.Token);
        }
        
        public async Task SendTypingSignal(string roomId, bool isTyping)
        {
            await _eventService.SendTypingSignalAsync(_accessToken!, roomId, UserId, isTyping, _cts.Token);
        }

        public async Task<string> GetRoomName(string roomId)
        {
            return await _roomService.GetRoomNameAsync(_accessToken!, roomId, _cts.Token);
        }
        
        public async Task<EventResponse> SetRoomTopicAsync(string roomId, string topic)
        {
            var transactionId = CreateTransactionId();
            return await _roomService.SetTopicAsync(_accessToken!, roomId, topic, _cts.Token);
        }
        public async Task<EventResponse> SetRoomAvatarAsync(string roomId, string url)
        {
            var transactionId = CreateTransactionId();
            return await _roomService.SetAvatarAsync(_accessToken!, roomId, url, _cts.Token);
        }
        public async Task<EventResponse> SetRoomNameAsync(string roomId, string name)
        {
            var transactionId = CreateTransactionId();
            return await _roomService.SetNameAsync(_accessToken!, roomId, name, _cts.Token);
        }
        public async Task<MatrixProfile> GetUserProfile(string userId)
        {
            return await _userService.GetProfile(_accessToken!, userId, _cts.Token);
        }

        public async Task<byte[]> GetMxcImage(string mxcUrl)
        {
            return await _mediaService.GetMedia(_accessToken!, mxcUrl, _cts.Token);
        }
    }
}