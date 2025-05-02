using Auctions.Service.API.GrpcServices.Client;
using Auctions.Service.API.ServiceContracts.Moderator;
using Auctions.Service.API.ServiceContracts.Participant;
using Auctions.Service.API.Services.Moderator;
using Auctions.Service.API.Services.Participant;
using BidMasterOnline.Core;
using BidMasterOnline.Infrastructure;

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
                    options.Authority = builder.Configuration["IdentityServer:Authority"],
                    options.TokenValidationParameters = new()
                    {
                        ValidateAudience = true,
                    };
                });

builder.Services.AddInfrastructure(builder.Configuration)
    .AddCoreServices();

builder.Services.AddScoped<IModeratorAuctionRequestsService, ModeratorAuctionRequestsService>();

builder.Services.AddScoped<IAuctionRequestsService, AuctionRequestsService>();
builder.Services.AddScoped<IAuctionsService, AuctionsService>();

builder.Services.AddScoped<ModerationClient>();
builder.Services.AddScoped<BidsClient>();

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
