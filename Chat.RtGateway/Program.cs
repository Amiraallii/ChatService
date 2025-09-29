using Chat.RtGateway.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var allowed = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "https://localhost:5173" };
builder.Services.AddCors(o => o.AddPolicy("SignalR", p =>
    p.WithOrigins(allowed).AllowAnyHeader().AllowAnyMethod().AllowCredentials()
));
builder.Services.AddSignalR().AddMessagePackProtocol();

builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
        ValidAudience = builder.Configuration["JwtConfig:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]!))
    };
    o.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
    {
        OnMessageReceived = ctx =>
        {
            var accessToken = ctx.Request.Query["access_token"];
            var path = ctx.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
                ctx.Token = accessToken;
            return Task.CompletedTask;
        }
    };
});

LogConfiguration.Configure(builder.Host);

var app = builder.Build();
app.UseCors("SignalR");
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/hubs/chat");
app.Run();

public class ChatHub : Microsoft.AspNetCore.SignalR.Hub { }
