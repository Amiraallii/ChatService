using Chat.Workers;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Error()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger);


builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
