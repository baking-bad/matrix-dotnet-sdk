namespace Matrix.Sdk.Core.Infrastructure.Dto.Event
{
    using System.Runtime.Serialization;

    public enum MessageType
    {
        [EnumMember(Value = "m.text")] Text
        // [JsonProperty("m.text")] Text
    }
}