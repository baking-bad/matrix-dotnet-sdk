namespace Matrix.Sdk.Core.Domain.RoomEvent
{
    using Infrastructure.Dto.Sync.Event;
    using Infrastructure.Dto.Sync.Event.Room;
    using Infrastructure.Dto.Sync.Event.Room.State;

    public record JoinRoomEvent(string EventId, string RoomId, string SenderUserId, DateTimeOffset Timestamp) : BaseRoomEvent(EventId, RoomId, SenderUserId, Timestamp)
    {
        public static class Factory
        {
            public static bool TryCreateFrom(RoomEvent roomEvent, string roomId, out JoinRoomEvent joinRoomEvent)
            {
                RoomMemberContent content = roomEvent.Content.ToObject<RoomMemberContent>();
                if (roomEvent.EventType == EventType.Member && content?.UserMembershipState == UserMembershipState.Join)
                {
                    joinRoomEvent = new JoinRoomEvent(roomEvent.EventId, roomId, roomEvent.SenderUserId, roomEvent.Timestamp);
                    return true;
                }

                joinRoomEvent = null;
                return false;
            }

            public static bool TryCreateFromStrippedState(RoomStrippedState roomStrippedState, string roomId,
                out JoinRoomEvent joinRoomEvent)
            {
                RoomMemberContent content = roomStrippedState.Content.ToObject<RoomMemberContent>();
                if (roomStrippedState.EventType == EventType.Member &&
                    content?.UserMembershipState == UserMembershipState.Join)
                {
                    joinRoomEvent = new JoinRoomEvent(string.Empty, roomId, roomStrippedState.SenderUserId, DateTimeOffset.MinValue);
                    return true;
                }

                joinRoomEvent = null;
                return false;
            }
        }
    }
}