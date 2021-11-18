namespace Matrix.Sdk.Core.Domain.RoomEvent
{
    public abstract record BaseRoomEvent(string RoomId, string SenderUserId);
}