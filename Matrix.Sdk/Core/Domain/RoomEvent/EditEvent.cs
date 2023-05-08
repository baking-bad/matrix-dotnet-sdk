using Matrix.Sdk.Core.Infrastructure.Dto.Event;
using Newtonsoft.Json;

namespace Matrix.Sdk.Core.Domain.RoomEvent
{
    public record EditEvent(MessageType MessageType, string Message, string EventId)
    {
        public string body = $"* {Message}";
        public MessageType msgtype { get; } = MessageType;

        [JsonProperty("m.new_content")] 
        public MessageEvent newContent = new MessageEvent(MessageType, Message);

        [JsonProperty("m.relates_to")] 
        public RelatesTo mRelatesTo = new RelatesTo(EventId);

        public record RelatesTo(string evntid)
        {
            public string event_id = evntid;
            public string rel_type = "m.replace";
        }
    }
}