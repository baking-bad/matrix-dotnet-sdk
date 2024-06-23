using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Matrix.Sdk.Core.Infrastructure.Extensions;
using Newtonsoft.Json;

namespace Matrix.Sdk.Core.Infrastructure.Services
{
    public class MediaService : BaseApiService
    {
        public MediaService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
        }
    
        private struct UploadResponse
        {
            public string content_uri;
        }
        
        public async Task<string> UploadImage(string accessToken, string filename, byte[] imageData, CancellationToken cancellationToken)
        {
            var extension = Path.GetExtension(filename);
            if (extension != ".png")
            {
                throw new Exception($"only png uploads are supported: {filename}");
            }
            
            string encodedFilename = HttpUtility.UrlEncode(filename);
            string url = $"{MediaPath}/upload?filename={encodedFilename}";
    
            HttpClient httpClient = CreateHttpClient(accessToken);
            using (var content = new ByteArrayContent(imageData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                content.Headers.ContentLength = imageData.Length;
                
                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UploadResponse>(json).content_uri;
            }
        }

        public async Task<string> UploadFile(string accessToken, string filename, byte[] imageData, CancellationToken cancellationToken)
        {
            var extension = Path.GetExtension(filename);
            if (string.IsNullOrEmpty(extension))
            {
                throw new Exception($"only uploads with filename and extension are supported: {filename}");
            }

            string encodedFilename = HttpUtility.UrlEncode(filename);
            string url = $"{MediaPath}/upload?filename={encodedFilename}";

            HttpClient httpClient = CreateHttpClient(accessToken);
            using (var content = new ByteArrayContent(imageData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/" + extension.Remove(0, 1));
                content.Headers.ContentLength = imageData.Length;

                var response = await httpClient.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UploadResponse>(json).content_uri;
            }
        }

        public async Task<byte[]> GetMedia(string accessToken, string mxcUrl, CancellationToken cancellationToken)
        {
            HttpClient httpClient = CreateHttpClient(accessToken);
    
            var validMxcStart = $"mxc://{httpClient.BaseAddress.Host}/";
            if (!mxcUrl.StartsWith(validMxcStart))
                throw new Exception($"Invalid mxc url: {mxcUrl}");
    
            var mxcId = mxcUrl.Substring(validMxcStart.Length);
            
            var path = $"{MediaPath}/download/{httpClient.BaseAddress.Host}/{mxcId}";
            return await httpClient.GetAsBytesAsync(path, cancellationToken);
        }
    }
}
