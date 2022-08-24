namespace Matrix.Sdk
{
    public static class Constants
    {
        public const string Matrix = nameof(Matrix);
        public const int FirstSyncTimout = 0;
        public const int LaterSyncTimout = 30000;

        public class EventType
        {
            public const string Create = "m.room.create";

            public const string Member = "m.room.member";

            public const string Message = "m.room.message";
        }

        public class MessageType
        {
            public const string Text = "m.text";
        }
    }
}