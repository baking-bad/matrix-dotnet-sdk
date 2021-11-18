namespace Matrix.Sdk.Core.Infrastructure.Dto.Room.Create
{
    using System.Runtime.Serialization;

    public enum Visibility
    {
        [EnumMember(Value = "public")] Public,

        [EnumMember(Value = "private")] Private
    }
}