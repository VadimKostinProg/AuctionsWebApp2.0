using BidMasterOnline.Infrastructure;
using BidMasterOnline.Core;
using Microsoft.OpenApi.Models;
using Users.Service.API.Filters;
using Quartz;
using Users.Service.API.BackgroundJobs;
using Users.Service.API.GrpcServices.Client;
using Users.Service.API.ServiceContracts;
using Users.Service.API.Services;

var builder = WebApplication.CreateBuilder(args);
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

builder.Services.AddScoped<Users.Service.API.ServiceContracts.Participant.IUserProfilesService, Users.Service.API.Services.Participant.UserProfilesService>();
builder.Services.AddScoped<Users.Service.API.ServiceContracts.Moderator.IUsersService, Users.Service.API.Services.Moderator.UsersService>();

builder.Services.AddScoped<INotificationsService, NotificationsSevice>();

builder.Services.AddScoped<UserAuctionsGrpcClient>();
builder.Services.AddScoped<UserBidsGrpcClient>();

builder.Services.AddQuartz(q =>
{
    JobKey jobKey = new("UnblockingUsersBackgroundJob");

    q.AddJob<UnblockingUsersBackgroundJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("MyJob-trigger")
        .WithCronSchedule("0 * * ? * *")
    );
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

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

        options.OAuthClientId("feedbacks-service-api-swagger");
        options.OAuthAppName("Feedbacks API - Swagger");
        options.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("CorsPolitics");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
