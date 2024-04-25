using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Matrix.Sdk.Core.Infrastructure.Dto.Event
{
    public record FileMessageEvent(string body, string url)
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType msgtype = MessageType.File;
    }
}
