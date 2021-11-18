namespace Matrix.Sdk.Core.Domain.RoomEvent
{
    using Infrastructure.Dto.Sync.Event;
    using Infrastructure.Dto.Sync.Event.Room;
    using Infrastructure.Dto.Sync.Event.Room.State;

    public record InviteToRoomEvent(string RoomId, string SenderUserId) : BaseRoomEvent(RoomId, SenderUserId)
    {
        public static class Factory
        {
            public static bool TryCreateFrom(RoomEvent roomEvent, string roomId,
                out InviteToRoomEvent inviteToRoomEvent)
            {
                RoomMemberContent content = roomEvent.Content.ToObject<RoomMemberContent>();
                if (roomEvent.EventType == EventType.Member &&
                    content?.UserMembershipState == UserMembershipState.Invite)
                {
                    inviteToRoomEvent = new InviteToRoomEvent(roomId, roomEvent.SenderUserId);
                    return true;
                }

                inviteToRoomEvent = new InviteToRoomEvent(string.Empty, string.Empty);
                return false;
            }

            public static bool TryCreateFromStrippedState(RoomStrippedState roomStrippedState, string roomId,
                out InviteToRoomEvent inviteToRoomEvent)
            {
                RoomMemberContent content = roomStrippedState.Content.ToObject<RoomMemberContent>();
                if (roomStrippedState.EventType == EventType.Member &&
                    content?.UserMembershipState == UserMembershipState.Invite)
                {
                    inviteToRoomEvent = new InviteToRoomEvent(roomId, roomStrippedState.SenderUserId);
                    return true;
                }

                inviteToRoomEvent = new InviteToRoomEvent(string.Empty, string.Empty);
                return false;
            }
        }
    }
}