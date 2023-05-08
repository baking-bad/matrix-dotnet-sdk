namespace Matrix.Sdk.Core.Domain.RoomEvent
{
    using Infrastructure.Dto.Sync.Event;
    using Infrastructure.Dto.Sync.Event.Room;
    using Infrastructure.Dto.Sync.Event.Room.State;

    public record RedactionEvent(string EventId, string RoomId, string SenderUserId, DateTimeOffset Timestamp, string Reason, string RedactsEventId) : BaseRoomEvent(EventId, RoomId, SenderUserId, Timestamp)
    {
        private record ReasonObj(string reason);
        
        public static class Factory
        {
            public static bool TryCreateFrom(RoomEvent roomEvent, string roomId, out RedactionEvent redactionEvent)
            {
                ReasonObj content = roomEvent.Content.ToObject<ReasonObj>();
                if (roomEvent.EventType == EventType.Redaction)
                {
                    redactionEvent = new RedactionEvent(roomEvent.EventId, roomId, roomEvent.SenderUserId, roomEvent.Timestamp, content.reason, roomEvent.redacts);
                    return true;
                }

                redactionEvent = null;
                return false;
            }

            public static bool TryCreateFromStrippedState(RoomStrippedState roomStrippedState, string roomId,
                out RedactionEvent redactionEvent)
            {
                ReasonObj content = roomStrippedState.Content.ToObject<ReasonObj>();
                if (roomStrippedState.EventType == EventType.Redaction)
                {
                    redactionEvent = new RedactionEvent(string.Empty, roomId, roomStrippedState.SenderUserId, DateTimeOffset.MinValue, content.reason, string.Empty);
                    return true;
                }

                redactionEvent = null;
                return false;
            }
        }
    }
}