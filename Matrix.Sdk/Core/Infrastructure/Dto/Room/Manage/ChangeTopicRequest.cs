namespace System.Runtime.CompilerServices.Dto.Room.Manage
{
        public record ChangeTopicRequest(string? topic )
        {
            public string? topic { get; } = topic;
        }
        
}