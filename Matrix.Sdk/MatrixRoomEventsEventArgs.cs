namespace Matrix.Sdk
{
    using System;
    using System.Collections.Generic;
    using Core.Domain.RoomEvent;

    public class MatrixRoomEventsEventArgs : EventArgs
    {
        public MatrixRoomEventsEventArgs(List<BaseRoomEvent> matrixRoomEvents, string nextBatch)
        {
            MatrixRoomEvents = matrixRoomEvents;
            NextBatch = nextBatch;
        }

        public List<BaseRoomEvent> MatrixRoomEvents { get; }
        
        public string NextBatch { get; }
    }
}