namespace Matrix.Sdk.Core.Infrastructure.Dto.Event
{
    using Newtonsoft.Json;

    public record MessageEvent(MessageType MessageType, string Message)
    {
        [JsonProperty("msgtype")] public MessageType MessageType { get; } = MessageType;

        [JsonProperty("body")] public string Message { get; } = Message;
    }

    public record MessageEvent2(string msgtype, string body)
    {
        public string msgtype { get; } = msgtype;

        public string body { get; } = body;
    }
}