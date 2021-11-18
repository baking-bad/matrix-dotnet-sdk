namespace Matrix.Sdk
{
    using System.Collections.Generic;
    using Core;
    using Core.Domain.RoomEvent;
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
            services.AddSingleton<ClientService>();

            services.AddHttpClient();

            services.AddSingleton<EventService>();
            services.AddSingleton<RoomService>();
            services.AddSingleton<UserService>();

            services.AddTransient<MatrixEventNotifier<List<BaseRoomEvent>>>();
            services.AddTransient<IPollingService, PollingService>();
            services.AddTransient<IMatrixClient, MatrixClient>();

            return services;
        }
    }
}