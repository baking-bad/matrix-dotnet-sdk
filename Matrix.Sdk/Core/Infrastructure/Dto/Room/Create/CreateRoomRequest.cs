namespace Matrix.Sdk.Core.Infrastructure.Dto.Room.Create
{
    public record CreateRoomRequest(
        Visibility? Visibility = null,
        string? RoomAliasName = null,
        string? Name = null,
        string? Topic = null,
        string[]? Invite = null,
        string? RoomVersion = null,
        Preset? Preset = null,
        bool? IsDirect = null);
}