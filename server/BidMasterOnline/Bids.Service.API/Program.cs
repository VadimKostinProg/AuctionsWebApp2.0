using BidMasterOnline.Core;
using BidMasterOnline.Infrastructure;
using Bids.Service.API.Filters;
using Bids.Service.API.GrpcServices.Client;
using Bids.Service.API.ServiceContracts.Moderator;
using Bids.Service.API.ServiceContracts.Participant;
using Bids.Service.API.Services.Moderator;
using Bids.Service.API.Services.Participant;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Bids.Service.API", Version = "v1" });

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
                    {"participantScope", "participantScope"},
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

builder.Services.AddScoped<IBidsPlacingStrategyFactory, BidsPlacingStrategyFactory>();
builder.Services.AddScoped<IParticipantBidsService, ParticipantBidsService>();
builder.Services.AddScoped<IModeratorBidsService, ModeratorBidsService>();

builder.Services.AddScoped<AuctionsGrpcClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");

        options.OAuthClientId("bids-service-api-swagger");
        options.OAuthAppName("Bids API - Swagger");
        options.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
