using Matrix.Sdk.Core.Domain.MatrixRoom;
using Matrix.Sdk.Core.Domain.RoomEvent;
using Matrix.Sdk.Core.Infrastructure.Dto.Sync.Event.Room;
using Matrix.Sdk.Core.Infrastructure.Services;
using Newtonsoft.Json;

namespace Matrix.Sdk
{
    public static class MatrixUtil
    {

        // factory bad
        private static readonly MatrixRoomEventFactory matrixRoomEventFactory = new();

        public static BaseRoomEvent Concretize(RoomEvent roomEvent)
        {
            var e = matrixRoomEventFactory.CreateFromJoined(roomEvent.RoomId, roomEvent);
            if (e == null)
            {
                e = matrixRoomEventFactory.CreateFromJoined(roomEvent.RoomId, roomEvent);
            }

            return e;
        }
    }
}