using System.Collections.Generic;
using System.Web;
using Matrix.Sdk.Core.Domain.RoomEvent;
using Matrix.Sdk.Core.Infrastructure.Dto.Sync.Event.Room;
using Newtonsoft.Json;

namespace Matrix.Sdk.Core.Infrastructure.Services
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.Event;
    using Dto.Sync;
    using Extensions;

    public class EventService : BaseApiService
    {
        public EventService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<SyncResponse> SyncAsync(string accessToken,
            CancellationToken cancellationToken,
            ulong? timeout = null, string? nextBatch = null)
        {
            HttpClient httpClient = CreateHttpClient(accessToken);
            
            var uri = new Uri($"{httpClient.BaseAddress}{ResourcePath}/sync");

            if (timeout != null)
                uri = uri.AddParameter("timeout", timeout.ToString());

            if (nextBatch != null)
                uri = uri.AddParameter("since", nextBatch);

            // HttpClient httpClient = CreateHttpClient(accessToken);

            return await httpClient.GetAsJsonAsync<SyncResponse>(uri.ToString(), cancellationToken);
        }

        public async Task<EventResponse> SendMessageAsync(string accessToken,
            string roomId, string transactionId,
            string message, CancellationToken cancellationToken)
        {
            const string eventType = "m.room.message";
            var model = new MessageEvent(MessageType.Text, message);

            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/send/{eventType}/{transactionId}";

            return await httpClient.PutAsJsonAsync<EventResponse>(path, model, cancellationToken);
        }
        
        public async Task<EventResponse> SendImageAsync(string accessToken,
            string roomId, string transactionId, string filename,
            string mxcUrl, CancellationToken cancellationToken)
        {
            var model = new ImageMessageEvent(filename, mxcUrl);
            
            const string eventType = "m.room.message";

            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/send/{eventType}/{transactionId}";

            var response = await httpClient.PutAsJsonAsync<EventResponse>(path, model, cancellationToken);
            return response;
        }    
        
        public async Task<EventResponse> SendFileAsync(string accessToken,
            string roomId, string transactionId, string filename,
            string mxcUrl, CancellationToken cancellationToken)
        {
            var model = new FileMessageEvent(filename, mxcUrl);
            
            const string eventType = "m.room.message";

            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/send/{eventType}/{transactionId}";

            var response = await httpClient.PutAsJsonAsync<EventResponse>(path, model, cancellationToken);
            return response;
        }

        public async Task<EventResponse> EditMessageAsync(string accessToken,
            string roomId, string transactionId, string eventId,
            string message, CancellationToken cancellationToken)
        {
            const string eventType = "m.room.message";
            var model = new EditEvent(MessageType.Text, message, eventId);

            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/send/{eventType}/{transactionId}";

            return await httpClient.PutAsJsonAsync<EventResponse>(path, model, cancellationToken);
        }
        
        public async Task SendTypingSignalAsync(string accessToken,
            string roomId, string userId, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var model = new TypingSignalEvent(true, (uint)timeout.TotalMilliseconds);
            await SendTypingSignalAsync(accessToken, roomId, userId, model, cancellationToken);
        }
        public async Task SendTypingSignalAsync(string accessToken,
            string roomId, string userId, bool isTyping, CancellationToken cancellationToken)
        {
            var model = new TypingSignalEvent(isTyping, 0);
            await SendTypingSignalAsync(accessToken, roomId, userId, model, cancellationToken);
        }
        public async Task SendTypingSignalAsync(string accessToken,
            string roomId, string userId, TypingSignalEvent typingEvent, CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(accessToken);

            var path = $"{ResourcePath}/rooms/{roomId}/typing/{userId}";
            await httpClient.PutAsJsonAsync<EventResponse>(path, typingEvent, cancellationToken);
        }
        
        private static readonly string RoomMessagesFilter = HttpUtility.UrlEncode(
            JsonConvert.SerializeObject(
                new Dictionary<string, bool>
                {
                    { "lazy_load_members", true }
                }));

        public struct RoomMessagesResponse
        {
            public struct Unsigned
            {
                public long age;
            }

            public string start;

            public RoomEventResponse[] chunk;
            public RoomEventResponse[] state;
            
            public string end;
        }
        
        public async Task<List<BaseRoomEvent>> GetTimelineEventsAsync(string accessToken, string roomId, string fromEventId, Func<BaseRoomEvent, Task<bool>> stopCallback, CancellationToken cancellationToken)
        {
            var events = new List<BaseRoomEvent>();

            bool hasHitFromEvent = false;
            if (fromEventId == null) hasHitFromEvent = true;
            string fromToken = null;
            while (true)
            {
                var messagesToPull = 1000;

                var path = $"{ResourcePath}/rooms/{roomId}/messages?limit={messagesToPull}&dir=b&filter={RoomMessagesFilter}";
                if (fromToken != null)
                {
                    path += $"&from={fromToken}";
                }

                HttpClient httpClient = CreateHttpClient(accessToken);
                var response = await httpClient.GetAsJsonAsync<RoomMessagesResponse>(path, cancellationToken);

                foreach (var roomEvent in response.chunk)
                {
                    if (roomEvent.EventId == fromEventId)
                    {
                        hasHitFromEvent = true;
                    }

                    if (!hasHitFromEvent)
                    {
                        continue;
                    }

                    var ev = BaseRoomEvent.Create(roomEvent.RoomId, roomEvent);
                    if (ev != null)
                    {
                        events.Add(ev);
                    }
                    else
                    {
                        Console.WriteLine($"Unable to concretize event: {JsonConvert.SerializeObject(roomEvent, Formatting.Indented)}");
                    }

                    if (ev != null)
                    {
                        if (await stopCallback.Invoke(ev))
                        {
                            return events;
                        }
                    }
                }

                fromToken = response.end;

                if ((response.chunk == null || response.chunk.Length == 0) && (response.state == null || response.state.Length == 0))
                {
                    break;
                }
            }
            return events;
        }

        public async Task<string> GetString(string accessToken, string url, CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(accessToken);
            return await httpClient.GetAsStringAsync(url, cancellationToken);
        }
    }
}