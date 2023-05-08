namespace Matrix.Sdk.Core.Domain.RoomEvent
{
    public abstract record BaseRoomEvent(string EventId, string RoomId, string SenderUserId, DateTimeOffset Timestamp);
}