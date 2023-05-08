namespace Matrix.Sdk.Core.Infrastructure.Dto.Sync.Event.Room.Messaging
{
    using Newtonsoft.Json;

    public enum MessageType
    {
        Text,
        Image,
        Unknown
    }

    public record MessageContent : BaseMessageContent
    {
        public record RelatesTo
        {
            public string rel_type;
            public string event_id;
            public string key;
        }

        [JsonProperty("m.relates_to")] 
        public RelatesTo relatesTo;

        [JsonIgnore]
        public string ReplacesEventId
        {
            get
            {
                if (relatesTo != null && relatesTo.rel_type == "m.replaces")
                {
                    return relatesTo.event_id;
                }
                return string.Empty;
            }
        }
        
        [JsonIgnore]
        public string CleanBody
        {
            get
            {
                if (relatesTo != null && relatesTo.rel_type == "m.replaces")
                {
                    return Body.Substring(3);
                }
                return Body;
            }
        }

        public string url;
    }
}