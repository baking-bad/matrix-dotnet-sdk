namespace Matrix.Sdk.Core.Infrastructure.Dto.Sync.Event.Room.State
{
    using Newtonsoft.Json;

    public enum UserMembershipState
    {
        Invite,
        Join,
        Knock,
        Leave,
        Ban
    }

    /// <remarks>
    ///     m.room.member
    /// </remarks>
    public record RoomMemberContent

    {
        /// <summary>
        ///     The avatar URL for this user, if any.
        /// </summary>
        public string? AvatarUrl { get; init; }

        /// <summary>
        ///     The display name for this user, if any.
        /// </summary>
        [JsonProperty("displayname")]
        public string? UserDisplayName { get; init; }

        /// <summary>
        ///     <b>Required.</b> The membership state of the user. One of: ["invite", "join", "knock", "leave", "ban"]
        /// </summary>
        [JsonProperty("membership")]
        public UserMembershipState UserMembershipState { get; init; }

        /// <summary>
        ///     Flag indicating if the room containing this event was created with the intention of being a direct chat.
        /// </summary>
        [JsonProperty("is_direct")]
        public bool IsDirectChat { get; init; }
    }
}