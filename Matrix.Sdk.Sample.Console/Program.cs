namespace Matrix.Sdk.Sample.Console
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog;
    using Serilog.Sinks.SystemConsole.Themes;

    internal static class Program
    {
        private static readonly Sample Sample = new();
        private static readonly DependencyInjectionSample DependencyInjectionSample = new();
        
#pragma warning disable CA1416
        private static IHostBuilder CreateHostBuilder() => new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddLogging(loggingBuilder =>
                    loggingBuilder.AddSerilog(dispose: true));

                services.AddMatrixClient();
            }).UseConsoleLifetime();
        
#pragma warning restore CA1416

        
        private static async Task<int> Main(string[] args)
        {
            IHost host = CreateHostBuilder().Build();
            
            SystemConsoleTheme theme = LoggerSetup.SetupTheme();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: theme)
                .CreateLogger();

            // await SimpleExample.Run();
            await DependencyInjectionSample.Run(host.Services);

            return 0;
        }
    }
}