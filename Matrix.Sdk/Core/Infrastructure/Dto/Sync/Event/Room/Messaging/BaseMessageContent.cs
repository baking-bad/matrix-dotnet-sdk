using Newtonsoft.Json;

namespace Matrix.Sdk.Core.Infrastructure.Dto.Sync.Event.Room.Messaging;

public record BaseMessageContent()
{
    /// <summary>
    ///     <b>Required.</b> The textual representation of this message.
    /// </summary>
    public string Body { get; init; }
    
    public MessageType MessageType { get; private set; }

    /// <summary>
    ///     <b>Required.</b> The type of message, e.g. m.image, m.text
    /// </summary>
    [JsonProperty("msgtype")]
    public string Type
    {
        set => MessageType = value switch
        {
            Constants.MessageType.Text => MessageType.Text,
            Constants.MessageType.Image => MessageType.Image,
            _ => MessageType.Unknown
        };
    }
}