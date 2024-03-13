namespace Matrix.Sdk.Core.Domain.RoomEvent {
    public record TypingSignalEvent(bool typing, uint timeout = 0);
}