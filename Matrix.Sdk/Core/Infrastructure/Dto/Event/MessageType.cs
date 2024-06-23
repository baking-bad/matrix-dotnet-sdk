namespace Matrix.Sdk.Core.Infrastructure.Dto.Event
{
    using System.Runtime.Serialization;

    public enum MessageType
    {
        [EnumMember(Value = "m.text")] Text,
        [EnumMember(Value = "m.image")] Image,
        [EnumMember(Value = "m.file")] File
    }
}