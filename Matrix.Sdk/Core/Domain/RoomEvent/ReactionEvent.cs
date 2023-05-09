using System;
using Matrix.Sdk.Core.Infrastructure.Dto.Sync.Event.Room.Messaging;

namespace Matrix.Sdk.Core.Domain.RoomEvent
{
    using Infrastructure.Dto.Sync.Event;
    using Infrastructure.Dto.Sync.Event.Room;
    using Infrastructure.Dto.Sync.Event.Room.State;
    
    public record ReactionEvent(string EventId, string RoomId, string SenderUserId, DateTimeOffset Timestamp, string Reaction, string RelatesToEventId) : BaseRoomEvent(EventId, RoomId, SenderUserId, Timestamp)
    {

        public static class Factory
        {
            public static bool TryCreateFrom(RoomEvent roomEvent, string roomId, out ReactionEvent reactionEvent)
            {
                MessageContent content = roomEvent.Content.ToObject<MessageContent>();
                if (roomEvent.EventType == EventType.Reaction && content?.relatesTo.rel_type == "m.annotation")
                {
                    reactionEvent = new ReactionEvent(roomEvent.EventId, roomId, roomEvent.SenderUserId, roomEvent.Timestamp, content.relatesTo.key, content.relatesTo.event_id);
                    return true;
                }

                reactionEvent = null;
                return false;
            }

            public static bool TryCreateFromStrippedState(RoomStrippedState roomStrippedState, string roomId,
                out ReactionEvent reactionEvent)
            {
                MessageContent content = roomStrippedState.Content.ToObject<MessageContent>();
                if (roomStrippedState.EventType == EventType.Reaction && content?.relatesTo.rel_type == "m.annotation")
                {
                    reactionEvent = new ReactionEvent(string.Empty, roomId, roomStrippedState.SenderUserId, DateTimeOffset.MinValue, content.relatesTo.key, content.relatesTo.event_id);
                    return true;
                }

                reactionEvent = null;
                return false;
            }
        }
    }
}