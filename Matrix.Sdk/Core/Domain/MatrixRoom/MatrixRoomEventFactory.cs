namespace Matrix.Sdk.Core.Domain.MatrixRoom
{
    using System.Collections.Generic;
    using Infrastructure.Dto.Sync;
    using Infrastructure.Dto.Sync.Event.Room;
    using RoomEvent;

    public class MatrixRoomEventFactory
    {
        public List<BaseRoomEvent> CreateFromJoined(string roomId, JoinedRoom joinedRoom)
        {
            var roomEvents = new List<BaseRoomEvent>();

            foreach (RoomEvent timelineEvent in joinedRoom.Timeline.Events)
                if (JoinRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out JoinRoomEvent joinRoomEvent))
                    roomEvents.Add(joinRoomEvent!);
                else if (CreateRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId,
                             out CreateRoomEvent createRoomEvent))
                    roomEvents.Add(createRoomEvent!);
                else if (InviteToRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId,
                             out InviteToRoomEvent inviteToRoomEvent))
                    roomEvents.Add(inviteToRoomEvent!);
                else if (TextMessageEvent.Factory.TryCreateFrom(timelineEvent, roomId,
                             out TextMessageEvent textMessageEvent))
                    roomEvents.Add(textMessageEvent);

            return roomEvents;
        }

        public List<BaseRoomEvent> CreateFromInvited(string roomId, InvitedRoom invitedRoom)
        {
            var roomEvents = new List<BaseRoomEvent>();

            foreach (RoomStrippedState inviteStateEvent in invitedRoom.InviteState.Events)
                if (JoinRoomEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId,
                        out JoinRoomEvent joinRoomEvent))
                    roomEvents.Add(joinRoomEvent!);
                else if (CreateRoomEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId,
                             out CreateRoomEvent createRoomEvent))
                    roomEvents.Add(createRoomEvent!);
                else if (InviteToRoomEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId,
                             out InviteToRoomEvent inviteToRoomEvent))
                    roomEvents.Add(inviteToRoomEvent!);
                else if (TextMessageEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId,
                             out TextMessageEvent textMessageEvent))
                    roomEvents.Add(textMessageEvent);

            return roomEvents;
        }

        public List<BaseRoomEvent> CreateFromLeft(string roomId, LeftRoom leftRoom)
        {
            var roomEvents = new List<BaseRoomEvent>();

            foreach (RoomEvent timelineEvent in leftRoom.Timeline.Events)
                if (JoinRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out JoinRoomEvent joinRoomEvent))
                    roomEvents.Add(joinRoomEvent!);
                else if (CreateRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId,
                             out CreateRoomEvent createRoomEvent))
                    roomEvents.Add(createRoomEvent!);
                else if (InviteToRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId,
                             out InviteToRoomEvent inviteToRoomEvent))
                    roomEvents.Add(inviteToRoomEvent!);
                else if (TextMessageEvent.Factory.TryCreateFrom(timelineEvent, roomId,
                             out TextMessageEvent textMessageEvent))
                    roomEvents.Add(textMessageEvent);

            return roomEvents;
        }
    }
}