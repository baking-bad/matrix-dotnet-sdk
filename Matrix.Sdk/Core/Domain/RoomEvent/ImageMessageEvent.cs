using System;

namespace Matrix.Sdk.Core.Domain.RoomEvent
{
    using Infrastructure.Dto.Sync.Event;
    using Infrastructure.Dto.Sync.Event.Room;
    using Infrastructure.Dto.Sync.Event.Room.Messaging;

    public record ImageMessageEvent(string EventId, string RoomId, string SenderUserId, DateTimeOffset Timestamp, string Message, string MxcUrl) : BaseRoomEvent(EventId, RoomId,
        SenderUserId, Timestamp)
    {
        public static class Factory
        {
            public static bool TryCreateFrom(RoomEventResponse roomEvent, string roomId, out ImageMessageEvent textMessageEvent)
            {
                ImageContent content = roomEvent.Content.ToObject<ImageContent>();
                if (roomEvent.EventType == EventType.Message && content?.MessageType == MessageType.Image)
                {
                    textMessageEvent = new ImageMessageEvent(roomEvent.EventId, roomId, roomEvent.SenderUserId, roomEvent.Timestamp, content.Body, content.url);
                    return true;
                }
                textMessageEvent = null;
                return false;
            }

            public static bool TryCreateFromStrippedState(RoomStrippedState roomStrippedState, string roomId,
                out ImageMessageEvent textMessageEvent)
            {
                ImageContent content = roomStrippedState.Content.ToObject<ImageContent>();
                if (roomStrippedState.EventType == EventType.Message && content?.MessageType == MessageType.Image)
                {
                    textMessageEvent = new ImageMessageEvent(string.Empty, roomId, roomStrippedState.SenderUserId, DateTimeOffset.MinValue, content.Body, content.url);
                    return true;
                }

                textMessageEvent = null;
                return false;
            }
        }
    }
}