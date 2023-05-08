namespace Matrix.Sdk.Core.Infrastructure.Dto.Event
{
    public record MessageEvent(MessageType MessageType, string Message)
    {
        public MessageType msgtype { get; } = MessageType;
        public string body { get; } = Message;
        public string formatted_body { get; } = Message == null ? null : Markdig.Markdown.ToHtml(Message);
        public string format = "org.matrix.custom.html";
    }
}