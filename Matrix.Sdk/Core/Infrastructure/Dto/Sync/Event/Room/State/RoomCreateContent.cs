namespace Matrix.Sdk.Core.Infrastructure.Dto.Sync.Event.Room.State
{
    using Newtonsoft.Json;

    /// <remarks>
    ///     m.room.create
    /// </remarks>
    public record RoomCreateContent
    {
        /// <summary>
        ///     <b>Required.</b> The user_id of the room creator.
        ///     This is set by the homeserver.
        /// </summary>
        [JsonProperty("creator")]
        public string RoomCreatorUserId { get; init; }

        /// <summary>
        ///     Whether users on other servers can join this room. Defaults to <b>true</b> if key does not exist.
        /// </summary>
        [JsonProperty("m.federate")]
        public bool Federate { get; init; }

        /// <summary>
        ///     The version of the room. Defaults to <b>"1"</b> if the key does not exist.
        /// </summary>
        public string? RoomVersion { get; init; }

        /// <summary>
        ///     A reference to the room this room replaces, if the previous room was upgraded.
        /// </summary>
        public PreviousRoom Predecessor { get; init; }
    }

    public record PreviousRoom
    {
        /// <summary>
        ///     <b>Required.</b> The ID of the old room.
        /// </summary>
        [JsonProperty("room_id")]
        public string OldRoomId { get; init; }

        /// <summary>
        ///     <b>Required.</b> The event ID of the last known event in the old room.
        /// </summary>
        [JsonProperty("event_id")]
        public string LastKnownEventId { get; init; }
    }
}