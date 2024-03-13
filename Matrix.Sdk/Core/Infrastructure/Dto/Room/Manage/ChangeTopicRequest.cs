namespace System.Runtime.CompilerServices.Dto.Room.Manage
{
        public record ChangeTopicRequest(string? topic )
        {
            public string? topic { get; } = topic;
        }

        public record ChangeNameRequest(string? name)
        {
            public string? name { get; } = name;
        }
        
        public record ChangeAvatarRequest(string? url)
        {
            public string? url { get; } = url;
        }
}