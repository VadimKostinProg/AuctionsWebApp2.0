using BidMasterOnline.Core;
using BidMasterOnline.Infrastructure;
using Microsoft.OpenApi.Models;
using Moderation.Service.API.Filters;
using Moderation.Service.API.ServiceContracts;
using Moderation.Service.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Moderation.Service.API", Version = "v1" });

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{builder.Configuration["IdentityServer:Authority"]}/connect/authorize"),
                TokenUrl = new Uri($"{builder.Configuration["IdentityServer:Authority"]}/connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    {"openid", "openid"},
                    {"profile", "profile"},
                    {"moderatorScope", "moderatorScope"}
                }
            }
        }
    });

    c.OperationFilter<AuthorizeCheckOperationFilter>();
});

builder.Services.AddGrpc();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = builder.Configuration["IdentityServer:Authority"];
                    options.TokenValidationParameters = new()
                    {
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["IdentityServer:Audience"]
                    };
                });

builder.Services.AddInfrastructure(builder.Configuration)
    .AddCoreServices();

builder.Services.AddScoped<IModerationLogsService, ModerationLogsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");

        options.OAuthClientId("moderation-service-api-swagger");
        options.OAuthAppName("Moderation API - Swagger");
        options.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
