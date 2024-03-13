using System;

namespace Matrix.Sdk.Core.Domain.RoomEvent
{
    using Infrastructure.Dto.Sync.Event;
    using Infrastructure.Dto.Sync.Event.Room;
    using Infrastructure.Dto.Sync.Event.Room.Messaging;

    public record TextMessageEvent(string EventId, string RoomId, string SenderUserId, DateTimeOffset Timestamp, string Message, string ReplacesEventId) : BaseRoomEvent(EventId, RoomId,
        SenderUserId, Timestamp)
    {
        public static class Factory
        {
            public static bool TryCreateFrom(RoomEventResponse roomEvent, string roomId, out TextMessageEvent textMessageEvent)
            {
                MessageContent content = roomEvent.Content.ToObject<MessageContent>();
                if (roomEvent.EventType == EventType.Message && content?.MessageType == MessageType.Text)
                {
                    textMessageEvent = new TextMessageEvent(roomEvent.EventId, roomId, roomEvent.SenderUserId, roomEvent.Timestamp, content.Body, content.ReplacesEventId);
                    return true;
                }

                textMessageEvent = new TextMessageEvent(roomEvent.EventId, string.Empty, string.Empty, roomEvent.Timestamp, string.Empty, string.Empty);
                return false;
            }

            public static bool TryCreateFromStrippedState(RoomStrippedState roomStrippedState, string roomId,
                out TextMessageEvent textMessageEvent)
            {
                MessageContent content = roomStrippedState.Content.ToObject<MessageContent>();
                if (roomStrippedState.EventType == EventType.Message && content?.MessageType == MessageType.Text)
                {
                    textMessageEvent = new TextMessageEvent(string.Empty, roomId, roomStrippedState.SenderUserId, DateTimeOffset.MinValue, content.Body, content.ReplacesEventId);
                    return true;
                }

                textMessageEvent = new TextMessageEvent(string.Empty, string.Empty, string.Empty, DateTimeOffset.MinValue, string.Empty, string.Empty);
                return false;
            }
        }
    }
}