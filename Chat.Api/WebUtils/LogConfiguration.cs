using Serilog;

namespace Chat.Api.WebUtils
{
    public static class LogConfiguration
    {
        public static void Configure(IHostBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Error()
                .CreateBootstrapLogger();

            builder.UseSerilog();
        }
    }
}
