namespace Matrix.Sdk.Core.Infrastructure.Dto.Room.Join
{
    public record JoinRoomResponse(string RoomId)
    {
        /// <summary>
        ///     <b>Required.</b> The joined room ID.
        /// </summary>
        public string RoomId { get; } = RoomId;
    }
}