namespace Matrix.Sdk.Core.Infrastructure.Services
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Dto.ClientVersion;
    using Extensions;

    public class ClientService : BaseApiService
    {
        public ClientService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }

        protected override string ResourcePath => "_matrix/client/versions";

        public async Task<MatrixServerVersionsResponse> GetMatrixClientVersions(Uri address,
            CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient();

            return await httpClient.GetAsJsonAsync<MatrixServerVersionsResponse>(ResourcePath, cancellationToken);
        }
    }
}