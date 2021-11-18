// ReSharper disable ArgumentsStyleNamedExpression

namespace Matrix.Sdk.Core.Infrastructure.Services
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.Login;
    using Extensions;

    public class UserService : BaseApiService
    {
        public UserService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        public async Task<LoginResponse> LoginAsync(string user, string password, string deviceId,
            CancellationToken cancellationToken)
        {
            var model = new LoginRequest
            (
                new Identifier
                (
                    "m.id.user",
                    user
                ),
                password,
                deviceId,
                "m.login.password"
            );

            HttpClient httpClient = CreateHttpClient();

            var path = $"{ResourcePath}/login";

            return await httpClient.PostAsJsonAsync<LoginResponse>(path, model, cancellationToken);
        }
    }
}