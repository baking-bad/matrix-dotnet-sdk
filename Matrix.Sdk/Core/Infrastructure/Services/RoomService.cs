using System.Collections.Generic;
using System.Runtime.CompilerServices.Dto.Room.Manage;
using Matrix.Sdk.Core.Infrastructure.Dto.Event;
using Newtonsoft.Json;

namespace Matrix.Sdk.Core.Infrastructure.Services
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.Room.Create;
    using Dto.Room.Join;
    using Dto.Room.Joined;
    using Extensions;

    public class RoomService : BaseApiService
    {
        public RoomService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<CreateRoomResponse> CreateRoomAsync(string accessToken, string[]? members,
            CancellationToken cancellationToken)
        {
            var model = new CreateRoomRequest
            (
                Invite: members,
                Preset: Preset.TrustedPrivateChat,
                IsDirect: true
            );

            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/createRoom";

            return await httpClient.PostAsJsonAsync<CreateRoomResponse>(path, model, cancellationToken);
        }

        public async Task<JoinRoomResponse> JoinRoomAsync(string accessToken, string roomId,
            CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/join";

            return await httpClient.PostAsJsonAsync<JoinRoomResponse>(path, null, cancellationToken);
        }


        public async Task<List<string>> GetJoinedRoomsAsync(string accessToken,
            CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(accessToken);
            
            var path = $"{ResourcePath}/joined_rooms";
            var json = await httpClient.GetAsStringAsync(path, cancellationToken);
            var obj = JsonConvert.DeserializeObject<JoinedRoomsResponse>(json);
            return obj.JoinedRoomIds;
        }

        public async Task LeaveRoomAsync(string accessToken, string roomId,
            CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/leave";

            await httpClient.PostAsync(path, cancellationToken);
        }

        public record RoomNameResponse
        {
            public string name;
        }
        public async Task<string> GetRoomNameAsync(string accessToken, string roomId, CancellationToken cancellationToken)
        {
            var path = $"{ResourcePath}/rooms/{roomId}/state/m.room.name/";
            HttpClient httpClient = CreateHttpClient(accessToken);
            var json = await httpClient.GetAsStringAsync(path, cancellationToken);
            var payload = JsonConvert.DeserializeObject<RoomNameResponse>(json);
            return payload.name;
        }
        
        public async Task<EventResponse> SetTopicAsync(string accessToken,
            string roomId,
            string topic, CancellationToken cancellationToken)
        {
            const string eventType = "m.room.topic";
            var model = new ChangeTopicRequest(topic);

            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/state/{eventType}";
            
            return await httpClient.PutAsJsonAsync<EventResponse>(path, model, cancellationToken);
        }
        
        public async Task<EventResponse> SetNameAsync(string accessToken,
            string roomId,
            string name, CancellationToken cancellationToken)
        {
            const string eventType = "m.room.name";
            var model = new ChangeNameRequest(name);

            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/state/{eventType}";
            
            return await httpClient.PutAsJsonAsync<EventResponse>(path, model, cancellationToken);
        }
        
        public async Task<EventResponse> SetAvatarAsync(string accessToken,
            string roomId,
            string url, CancellationToken cancellationToken)
        {
            const string eventType = "m.room.avatar";
            var model = new ChangeAvatarRequest(url);

            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/state/{eventType}";
            
            return await httpClient.PutAsJsonAsync<EventResponse>(path, model, cancellationToken);
        }
    }
}