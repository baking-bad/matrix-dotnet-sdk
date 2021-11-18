namespace Matrix.Sdk
{
    using System;
    using System.Collections.Generic;
    using Core.Domain.RoomEvent;

    public class MatrixRoomEventsEventArgs : EventArgs
    {
        public MatrixRoomEventsEventArgs(List<BaseRoomEvent> matrixRoomEvents)
        {
            MatrixRoomEvents = matrixRoomEvents;
        }

        public List<BaseRoomEvent> MatrixRoomEvents { get; }
    }
}