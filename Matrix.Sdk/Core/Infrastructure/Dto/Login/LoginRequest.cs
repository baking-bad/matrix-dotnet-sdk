namespace Matrix.Sdk.Core.Infrastructure.Dto.Login
{
    public record LoginRequest(Identifier Identifier, string Password, string DeviceId, string Type)
    {
        public Identifier Identifier { get; } = Identifier;
        public string Password { get; } = Password;
        public string DeviceId { get; } = DeviceId;
        public string Type { get; } = Type;
    }
}