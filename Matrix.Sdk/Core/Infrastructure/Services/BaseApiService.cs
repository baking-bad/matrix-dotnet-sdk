namespace Matrix.Sdk.Core.Infrastructure.Services
{
    using System;
    using System.Net.Http;
    using Extensions;

    public abstract class BaseApiService
    {
        // see: https://github.com/dotnet/aspnetcore/issues/28385#issuecomment-853766480
        private readonly IHttpClientFactory _httpClientFactory;
        
        public Uri? BaseAddress { get; set; }
        
        protected BaseApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected virtual string ResourcePath => "_matrix/client/r0";


        /// <summary>
        ///     Creates HttpClient
        /// </summary>
        /// <param name="accessToken">User access token.</param>
        /// <returns>HttpClient</returns>
        protected HttpClient CreateHttpClient(string? accessToken = null)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient(Constants.Matrix);

            if (accessToken != null)
                httpClient.AddBearerToken(accessToken);

            if (BaseAddress == null) 
                throw new NullReferenceException("set base address");
            
            if (httpClient.BaseAddress == null) 
                httpClient.BaseAddress = BaseAddress;

            return httpClient;
        }
    }
}