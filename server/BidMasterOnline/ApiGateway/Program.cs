using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
      .SetBasePath(builder.Environment.ContentRootPath)
      .AddOcelot();

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

//app.UseAuthentication();
//app.UseAuthorization();

await app.UseOcelot();

await app.RunAsync();
