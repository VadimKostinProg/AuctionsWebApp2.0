using BidMasterOnline.Core;
using BidMasterOnline.Infrastructure;
using Feedbacks.Service.API.Filters;
using Feedbacks.Service.API.GrpcServices.Client;
using Feedbacks.Service.API.ServiceContracts.Moderator;
using Feedbacks.Service.API.ServiceContracts.Participant;
using Feedbacks.Service.API.Services.Moderator;
using Feedbacks.Service.API.Services.Participant;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Feedbacks.Service.API", Version = "v1" });

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

builder.Services.AddScoped<ModerationClient>();
builder.Services.AddScoped<IParticipantAuctionCommentsService, ParticipantAuctionCommentsService>();
builder.Services.AddScoped<IParticipantComplaintsService, ParticipantComplaintsService>();
builder.Services.AddScoped<IParticipantSupportTicketsService, ParticipantSupportTicketsService>();
builder.Services.AddScoped<IParticipantUserFeedbacksService, ParticipantUserFeedbacksService>();
builder.Services.AddScoped<IModeratorAuctionCommentsService, ModeratorAuctionCommentsService>();
builder.Services.AddScoped<IModeratorComplaintsService, ModeratorComplaintsService>();
builder.Services.AddScoped<IModeratorSupportTicketsService, ModeratorSupportTicketsService>();
builder.Services.AddScoped<IModeratorUserFeedbacksService, ModeratorUserFeedbacksService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");

        options.OAuthClientId("feedbacks-service-api-swagger");
        options.OAuthAppName("Feedbacks API - Swagger");
        options.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
