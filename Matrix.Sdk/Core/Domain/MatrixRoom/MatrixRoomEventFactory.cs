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
            {
                var e = CreateFromJoined(roomId, timelineEvent);
                if (e != null) roomEvents.Add(e);

            }
            return roomEvents;
        }
        
        public BaseRoomEvent CreateFromJoined(string roomId, RoomEvent timelineEvent)
        {
            if (JoinRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out JoinRoomEvent joinRoomEvent)) return joinRoomEvent;
            if (CreateRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var createRoomEvent)) return createRoomEvent;
            if (InviteToRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var inviteToRoomEvent)) return inviteToRoomEvent;
            if (TextMessageEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var textMessageEvent)) return textMessageEvent;
            if (ImageMessageEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var imageMessageEvent)) return imageMessageEvent;
            if (RedactionEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var redactionEvent)) return redactionEvent;
            if (ReactionEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var reactionEvent)) return reactionEvent;
            return null;
        }

        public List<BaseRoomEvent> CreateFromInvited(string roomId, InvitedRoom invitedRoom)
        {
            var roomEvents = new List<BaseRoomEvent>();

            foreach (RoomStrippedState inviteStateEvent in invitedRoom.InviteState.Events)
            {
                var e = CreateFromInvited(roomId, inviteStateEvent);
                if (e != null)
                {
                    roomEvents.Add(e);
                }
            }
            return roomEvents;
        }
        
        public BaseRoomEvent CreateFromInvited(string roomId, RoomStrippedState inviteStateEvent)
        {
            if (JoinRoomEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var joinRoomEvent)) return joinRoomEvent;
            if (CreateRoomEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var createRoomEvent)) return createRoomEvent;
            if (InviteToRoomEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var inviteToRoomEvent)) return inviteToRoomEvent;
            if (TextMessageEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var textMessageEvent)) return textMessageEvent;
            if (ImageMessageEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var imageMessageEvent)) return imageMessageEvent;
            if (RedactionEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var redactionEvent)) return redactionEvent;
            if (ReactionEvent.Factory.TryCreateFromStrippedState(inviteStateEvent, roomId, out var reactionEvent)) return reactionEvent;
            return null;
        }


        public List<BaseRoomEvent> CreateFromLeft(string roomId, LeftRoom leftRoom)
        {
            var roomEvents = new List<BaseRoomEvent>();

            foreach (RoomEvent timelineEvent in leftRoom.Timeline.Events)
            {
                var e = CreateFromLeft(roomId, timelineEvent);
                if (e != null) roomEvents.Add(e);
            }
            return roomEvents;
        }
        
        public BaseRoomEvent CreateFromLeft(string roomId, RoomEvent timelineEvent)
        {
            if (JoinRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out JoinRoomEvent joinRoomEvent)) return joinRoomEvent;
            if (CreateRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out CreateRoomEvent createRoomEvent)) return createRoomEvent;
            if (InviteToRoomEvent.Factory.TryCreateFrom(timelineEvent, roomId, out InviteToRoomEvent inviteToRoomEvent)) return inviteToRoomEvent;
            if (TextMessageEvent.Factory.TryCreateFrom(timelineEvent, roomId, out TextMessageEvent textMessageEvent)) return textMessageEvent;
            if (RedactionEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var redactionEvent)) return redactionEvent;
            if (ReactionEvent.Factory.TryCreateFrom(timelineEvent, roomId, out var reactionEvent)) return reactionEvent;
            return null;
        }
    }
}