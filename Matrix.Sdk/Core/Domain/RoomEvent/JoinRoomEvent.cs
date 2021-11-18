namespace Matrix.Sdk.Core.Domain.RoomEvent
{
    using Infrastructure.Dto.Sync.Event;
    using Infrastructure.Dto.Sync.Event.Room;
    using Infrastructure.Dto.Sync.Event.Room.State;

    public record JoinRoomEvent(string RoomId, string SenderUserId) : BaseRoomEvent(RoomId, SenderUserId)
    {
        public static class Factory
        {
            public static bool TryCreateFrom(RoomEvent roomEvent, string roomId, out JoinRoomEvent joinRoomEvent)
            {
                RoomMemberContent content = roomEvent.Content.ToObject<RoomMemberContent>();
                if (roomEvent.EventType == EventType.Member && content?.UserMembershipState == UserMembershipState.Join)
                {
                    joinRoomEvent = new JoinRoomEvent(roomId, roomEvent.SenderUserId);
                    return true;
                }

                joinRoomEvent = new JoinRoomEvent(string.Empty, string.Empty);
                return false;
            }

            public static bool TryCreateFromStrippedState(RoomStrippedState roomStrippedState, string roomId,
                out JoinRoomEvent joinRoomEvent)
            {
                RoomMemberContent content = roomStrippedState.Content.ToObject<RoomMemberContent>();
                if (roomStrippedState.EventType == EventType.Member &&
                    content?.UserMembershipState == UserMembershipState.Join)
                {
                    joinRoomEvent = new JoinRoomEvent(roomId, roomStrippedState.SenderUserId);
                    return true;
                }

                joinRoomEvent = new JoinRoomEvent(string.Empty, string.Empty);
                return false;
            }
        }
    }
}