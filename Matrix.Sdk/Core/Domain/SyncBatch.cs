namespace Matrix.Sdk.Core.Domain
{
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Dto.Sync;
    using Room;
    using RoomEvent;

    public record SyncBatch
    {
        private SyncBatch(string nextBatch, List<Room.MatrixRoom> matrixRooms,
            List<BaseRoomEvent> matrixRoomEvents)
        {
            NextBatch = nextBatch;
            MatrixRooms = matrixRooms;
            MatrixRoomEvents = matrixRoomEvents;
        }

        public string NextBatch { get; }
        public List<Room.MatrixRoom> MatrixRooms { get; }
        public List<BaseRoomEvent> MatrixRoomEvents { get; }

        internal static class Factory
        {
            public static SyncBatch CreateFromSync(string nextBatch, Rooms rooms)
            {
                List<Room.MatrixRoom> matrixRooms = GetMatrixRoomsFromSync(rooms);
                List<BaseRoomEvent> matrixRoomEvents = GetMatrixEventsFromSync(rooms);

                return new SyncBatch(nextBatch, matrixRooms, matrixRoomEvents);
            }

            private static List<Room.MatrixRoom> GetMatrixRoomsFromSync(Rooms rooms)
            {
                var joinedMatrixRooms = rooms.Join.Select(pair => Room.MatrixRoom.Create(pair.Key, pair.Value, MatrixRoomStatus.Joined))
                    .ToList();
                var invitedMatrixRooms = rooms.Invite
                    .Select(pair => Room.MatrixRoom.CreateInvite(pair.Key, pair.Value)).ToList();
                var leftMatrixRooms = rooms.Leave.Select(pair => Room.MatrixRoom.Create(pair.Key, pair.Value, MatrixRoomStatus.Left))
                    .ToList();

                return joinedMatrixRooms.Concat(invitedMatrixRooms).Concat(leftMatrixRooms).ToList();
            }

            private static List<BaseRoomEvent> GetMatrixEventsFromSync(Rooms rooms)
            {
                var joinedMatrixRoomEvents = rooms.Join
                    .SelectMany(pair => BaseRoomEvent.Create(pair.Key, pair.Value)).ToList();
                var invitedMatrixRoomEvents = rooms.Invite
                    .SelectMany(pair => BaseRoomEvent.CreateFromInvited(pair.Key, pair.Value)).ToList();
                var leftMatrixRoomEvents = rooms.Leave
                    .SelectMany(pair => BaseRoomEvent.Create(pair.Key, pair.Value)).ToList();

                return joinedMatrixRoomEvents.Concat(invitedMatrixRoomEvents).Concat(leftMatrixRoomEvents).ToList();
            }
        }
    }
}