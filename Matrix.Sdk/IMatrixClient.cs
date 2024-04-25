using System.Runtime.CompilerServices.Dto.User;
using Matrix.Sdk.Core.Domain.RoomEvent;
using Matrix.Sdk.Core.Infrastructure.Dto.Event;
using Matrix.Sdk.Core.Infrastructure.Dto.Sync;
using Matrix.Sdk.Core.Infrastructure.Dto.Sync.Event.Room;
using Matrix.Sdk.Core.Infrastructure.Services;

namespace Matrix.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Domain.Room;
    using Core.Infrastructure.Dto.Room.Create;
    using Core.Infrastructure.Dto.Room.Join;

    /// <summary>
    ///     A Client for interaction with Matrix.
    /// </summary>
    public interface IMatrixClient
    {
        string UserId { get; }

        Uri? BaseAddress { get; }

        bool IsLoggedIn { get; }
        
        bool IsSyncing { get; }

        MatrixRoom[] InvitedRooms { get; }

        MatrixRoom[] JoinedRooms { get; }

        MatrixRoom[] LeftRooms { get; }
        
        event EventHandler<MatrixRoomEventsEventArgs> OnMatrixRoomEventsReceived;

        Task LoginAsync(Uri baseAddress, string user, string password, string deviceId);
        Task LoginAsync(Uri baseAddress, string token, string? userId = null);

        void Start(string? nextBatch = null);

        void Stop();

        Task<CreateRoomResponse> CreateTrustedPrivateRoomAsync(string[] invitedUserIds);

        Task<JoinRoomResponse> JoinTrustedPrivateRoomAsync(string roomId);

        Task<string> SendMessageAsync(string roomId, string message);
        Task<string> SendImageAsync(string roomId, string filename, byte[] imageData);
        Task<string> SendFileAsync(string roomId, string filename, byte[] blob);

        Task<List<string>> GetJoinedRoomsIdsAsync();

        Task LeaveRoomAsync(string roomId);
        
        Task<List<BaseRoomEvent>> GetHistory(string roomId, Func<BaseRoomEvent, Task<bool>> stopCallback);
        Task<List<BaseRoomEvent>> GetHistory(string roomId, string fromEventId, Func<BaseRoomEvent, Task<bool>> stopCallback);

        Task<string> EditMessage(string roomId, string messageId, string newText);
        Task SendTypingSignal(string roomId, TimeSpan timeout);
        Task SendTypingSignal(string roomId, bool isTyping);

        Task<string> GetString(string url);
        Task<string> GetRoomName(string id);
        Task<EventResponse> SetRoomTopicAsync(string roomId, string topic);
        Task<EventResponse> SetRoomNameAsync(string roomId, string name);
        Task<EventResponse> SetRoomAvatarAsync(string roomId, string url);
        Task<MatrixProfile> GetUserProfile(string fullUserId);
        Task<byte[]> GetMxcImage(string mxcUrl);
    }
}