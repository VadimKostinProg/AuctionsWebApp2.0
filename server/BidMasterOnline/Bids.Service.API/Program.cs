using BidMasterOnline.Core;
using BidMasterOnline.Infrastructure;
using Bids.Service.API.GrpcServices.Client;
using Bids.Service.API.ServiceContracts.Moderator;
using Bids.Service.API.ServiceContracts.Participant;
using Bids.Service.API.Services.Moderator;
using Bids.Service.API.Services.Participant;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGrpc();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = builder.Configuration["IdentityServer:Authority"];
                    options.TokenValidationParameters = new()
                    {
                        ValidateAudience = true,
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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
