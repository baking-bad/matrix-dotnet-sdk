namespace Matrix.Sdk.Core.Infrastructure.Dto.Sync.Event
{
    using Newtonsoft.Json.Linq;

    public record BaseEvent
    {
        /// <summary>
        ///     <b>Required.</b> The type of event.
        ///     This SHOULD be namespaced similar to Java package naming conventions e.g. 'com.example.subdomain.event.type'
        /// </summary>
        public JObject Content { get; init; }

        /// <summary>
        ///     <b>Required.</b> The fields in this object will vary depending on the type of event.
        ///     When interacting with the REST API, this is the HTTP body.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public EventType EventType { get; private set; }

        // ReSharper disable once UnusedMember.Global
        public string Type
        {
            set => EventType = value switch
            {
                Constants.EventType.Create => EventType.Create,
                Constants.EventType.Member => EventType.Member,
                Constants.EventType.Message => EventType.Message,
                _ => EventType.Unknown
            };
        }
    }
}