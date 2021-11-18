namespace Matrix.Sdk.Core.Infrastructure.Dto.Login
{
    public record LoginResponse(string UserId, string AccessToken, string HomeServer, string DeviceId)
    {
        public string UserId { get; } = UserId;
        public string AccessToken { get; } = AccessToken;
        public string HomeServer { get; } = HomeServer;
        public string DeviceId { get; } = DeviceId;
    }
}