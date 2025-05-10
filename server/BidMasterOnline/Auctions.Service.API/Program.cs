using Auctions.Service.API.BackgroundJobs;
using Auctions.Service.API.Filters;
using Auctions.Service.API.GrpcServices.Client;
using Auctions.Service.API.ServiceContracts.Moderator;
using Auctions.Service.API.ServiceContracts.Participant;
using Auctions.Service.API.Services.Moderator;
using Auctions.Service.API.Services.Participant;
using BidMasterOnline.Core;
using BidMasterOnline.Infrastructure;
using Microsoft.OpenApi.Models;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Auctions.Service.API", Version = "v1" });

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

builder.Services.AddScoped<IModeratorAuctionRequestsService, ModeratorAuctionRequestsService>();
builder.Services.AddScoped<IModeratorAuctionsService, ModeratorAuctionsService>();
builder.Services.AddScoped<IModeratorAuctionCategoriesService, ModeratorAuctionCategoriesService>();
builder.Services.AddScoped<IModeratorAuctionTypesService, ModeratorAuctionTypesService>();
builder.Services.AddScoped<IModeratorAuctionFinishMethodsService, ModeratorAuctionFinishMethodsService>();

builder.Services.AddScoped<IAuctionRequestsService, AuctionRequestsService>();
builder.Services.AddScoped<IParticipantAuctionsService, ParticipantAuctionsService>();
builder.Services.AddScoped<IParticipantAuctionCategoriesService, ParticipantAuctionCategoriesService>();
builder.Services.AddScoped<IParticipantAuctionTypesService, ParticipantAuctionTypesService>();
builder.Services.AddScoped<IParticipantAuctionFinishMethodsService, ParticipantAuctionFinishMethodsService>();

builder.Services.AddScoped<ModerationClient>();
builder.Services.AddScoped<BidsClient>();

builder.Services.AddQuartz(q =>
{
    JobKey jobKey = new("FinishingAuctionsBackgroundJob");

    q.AddJob<FinishingAuctionsBackgroundJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("MyJob-trigger")
        .WithCronSchedule("0 * * ? * *")
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");

        options.OAuthClientId("auctions-service-api-swagger");
        options.OAuthAppName("Auctions API - Swagger");
        options.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
