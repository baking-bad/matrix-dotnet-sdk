using System.Net.Http;

namespace Matrix.Sdk
{
    using Core.Domain.Services;
    using Core.Infrastructure.Services;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///     Extensions methods to configure an <see cref="IServiceCollection" /> for <see cref="IHttpClientFactory" /> with
    ///     Matrix Sdk.
    /// </summary>
    public static class MatrixClientServiceExtensions
    {
        public static IServiceCollection AddMatrixClient(this IServiceCollection services)
        {
            services.AddSingleton<IHttpClientFactory, SingletonHttpFactory>();

            services.AddSingleton<ClientService>();
            services.AddSingleton<EventService>();
            services.AddSingleton<RoomService>();
            services.AddSingleton<UserService>();
            services.AddSingleton<MediaService>(); 
            services.AddTransient<IPollingService, PollingService>();
            services.AddTransient<IMatrixClient, MatrixClient>();

            return services;
        }
    }
}