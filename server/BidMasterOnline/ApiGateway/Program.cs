using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
      .SetBasePath(builder.Environment.ContentRootPath)
      .AddOcelot();

builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["IdentityServer:Authority"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = builder.Configuration["IdentityServer:Audience"]
        };
    });

var app = builder.Build();

await app.UseOcelot();

await app.RunAsync();
