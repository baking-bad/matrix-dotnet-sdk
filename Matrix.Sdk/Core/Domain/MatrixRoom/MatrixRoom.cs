namespace Matrix.Sdk.Core.Domain.MatrixRoom
{
    using System.Collections.Generic;

    public record MatrixRoom
    {
        public MatrixRoom(string id, MatrixRoomStatus status, List<string> joinedUserIds)
        {
            Id = id;
            Status = status;
            JoinedUserIds = joinedUserIds;
        }

        public MatrixRoom(string id, MatrixRoomStatus status)
        {
            Id = id;
            Status = status;
            JoinedUserIds = new List<string>();
        }

        public string Id { get; }

        public MatrixRoomStatus Status { get; }

        public List<string> JoinedUserIds { get; }
    }
}