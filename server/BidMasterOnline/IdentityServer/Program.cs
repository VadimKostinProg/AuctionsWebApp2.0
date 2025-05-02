using IdentityServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureIdentityServer(builder.Configuration);

var app = builder.Build();

app.UseRouting();
app.UseIdentityServer();

app.Run();
