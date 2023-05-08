using Matrix.Sdk.Core.Domain.MatrixRoom;
using Matrix.Sdk.Core.Domain.RoomEvent;
using Matrix.Sdk.Core.Infrastructure.Dto.Sync.Event.Room;
using Matrix.Sdk.Core.Infrastructure.Services;
using Newtonsoft.Json;

namespace Matrix.Sdk;

public static class MatrixUtil
{
    
    // public static BaseRoomEvent FromChunk(RoomService.RoomMessagesResponse.Chunk chunk)
    // {
    //     var json = JsonConvert.SerializeObject(chunk);
    //     var roomEvent = JsonConvert.DeserializeObject<RoomEvent>(json);
    //     return ConcretizeEvent(chunk.room_id, roomEvent);
    // }
    
    // factory bad
    private static readonly MatrixRoomEventFactory matrixRoomEventFactory = new();
    // private static BaseRoomEvent ConcretizeEvent(string roomId, RoomEvent ev)
    // {
    //     var e = matrixRoomEventFactory.CreateFromJoined(roomId, ev);
    //     if (e == null)
    //     {
    //         e = matrixRoomEventFactory.CreateFromLeft(roomId, ev);
    //     }
    //     return e;
    // }

    public static BaseRoomEvent Concretize(RoomEvent roomEvent)
    {
        var e = matrixRoomEventFactory.CreateFromJoined(roomEvent.RoomId, roomEvent);
        if (e == null)
        {
            e = matrixRoomEventFactory.CreateFromLeft(roomEvent.RoomId, roomEvent);
        }
        return e;
    }
}