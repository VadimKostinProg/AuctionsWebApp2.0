using Auctions.Service.API.BackgroundJobs;
using Auctions.Service.API.Filters;
using Auctions.Service.API.GrpcServices.Client;
using Auctions.Service.API.GrpcServices.Server;
using BidMasterOnline.Core;
using BidMasterOnline.Infrastructure;
using Microsoft.OpenApi.Models;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName);

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

builder.Services.AddScoped<Auctions.Service.API.ServiceContracts.Moderator.IAuctionRequestsService, Auctions.Service.API.Services.Moderator.AuctionRequestsService>();
builder.Services.AddScoped<Auctions.Service.API.ServiceContracts.Moderator.IAuctionsService, Auctions.Service.API.Services.Moderator.AuctionsService>();
builder.Services.AddScoped<Auctions.Service.API.ServiceContracts.Moderator.IAuctionCategoriesService, Auctions.Service.API.Services.Moderator.AuctionCategoriesService>();
builder.Services.AddScoped<Auctions.Service.API.ServiceContracts.Moderator.IAuctionTypesService, Auctions.Service.API.Services.Moderator.AuctionTypesService>();
builder.Services.AddScoped<Auctions.Service.API.ServiceContracts.Moderator.IAuctionFinishMethodsService, Auctions.Service.API.Services.Moderator.AuctionFinishMethodsService>();

builder.Services.AddScoped<Auctions.Service.API.ServiceContracts.Participant.IAuctionRequestsService, Auctions.Service.API.Services.Participant.AuctionRequestsService>();
builder.Services.AddScoped<Auctions.Service.API.ServiceContracts.Participant.IAuctionsService, Auctions.Service.API.Services.Participant.AuctionsService>();
builder.Services.AddScoped<Auctions.Service.API.ServiceContracts.Participant.IAuctionCategoriesService, Auctions.Service.API.Services.Participant.AuctionCategoriesService>();
builder.Services.AddScoped<Auctions.Service.API.ServiceContracts.Participant.IAuctionTypesService, Auctions.Service.API.Services.Participant.AuctionTypesService>();
builder.Services.AddScoped<Auctions.Service.API.ServiceContracts.Participant.IAuctionFinishMethodsService, Auctions.Service.API.Services.Participant.AuctionFinishMethodsService>();

builder.Services.AddScoped<ModerationGrpcClient>();
builder.Services.AddScoped<BidsGrpcClient>();

builder.Services.AddQuartz(q =>
{
    JobKey jobKey = new("AuctionsStatusCheckBackgroundJob");

    q.AddJob<AuctionsStatusCheckBackgroundJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("MyJob-trigger")
        .WithCronSchedule("0 * * ? * *")
    );
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolitics",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers();

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

app.UseCors("CorsPolitics");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<AuctionsGrpcService>();
app.MapGrpcService<UserAuctionsGrpcService>();

app.Run();
