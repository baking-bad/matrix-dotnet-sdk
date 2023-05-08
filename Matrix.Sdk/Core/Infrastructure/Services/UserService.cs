// ReSharper disable ArgumentsStyleNamedExpression

using System.Runtime.CompilerServices.Dto.User;
using System.Text;
using System.Web;

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

        public async Task<MatrixProfile> GetProfile(string accessToken, string userId, CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(accessToken);
            var path = $"{ResourcePath}/profile/{HttpUtility.HtmlEncode($"@{userId}:{httpClient.BaseAddress.Host}")}";
            return await httpClient.GetAsJsonAsync<MatrixProfile>(path, cancellationToken);
        }
    }
}