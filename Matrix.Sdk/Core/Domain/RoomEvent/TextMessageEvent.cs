namespace Matrix.Sdk.Core.Domain.RoomEvent
{
    using Infrastructure.Dto.Sync.Event;
    using Infrastructure.Dto.Sync.Event.Room;
    using Infrastructure.Dto.Sync.Event.Room.Messaging;

    public record TextMessageEvent(string RoomId, string SenderUserId, string Message) : BaseRoomEvent(RoomId,
        SenderUserId)
    {
        public static class Factory
        {
            public static bool TryCreateFrom(RoomEvent roomEvent, string roomId, out TextMessageEvent textMessageEvent)
            {
                MessageContent content = roomEvent.Content.ToObject<MessageContent>();
                if (roomEvent.EventType == EventType.Message && content?.MessageType == MessageType.Text)
                {
                    textMessageEvent = new TextMessageEvent(roomId, roomEvent.SenderUserId, content.Body);
                    return true;
                }

                textMessageEvent = new TextMessageEvent(string.Empty, string.Empty, string.Empty);
                return false;
            }

            public static bool TryCreateFromStrippedState(RoomStrippedState roomStrippedState, string roomId,
                out TextMessageEvent textMessageEvent)
            {
                MessageContent content = roomStrippedState.Content.ToObject<MessageContent>();
                if (roomStrippedState.EventType == EventType.Message && content?.MessageType == MessageType.Text)
                {
                    textMessageEvent = new TextMessageEvent(roomId, roomStrippedState.SenderUserId, content.Body);
                    return true;
                }

                textMessageEvent = new TextMessageEvent(string.Empty, string.Empty, string.Empty);
                return false;
            }
        }
    }
}