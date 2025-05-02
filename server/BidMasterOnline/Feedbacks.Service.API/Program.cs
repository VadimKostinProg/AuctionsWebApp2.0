using BidMasterOnline.Core;
using BidMasterOnline.Infrastructure;
using Feedbacks.Service.API.GrpcServices.Client;
using Feedbacks.Service.API.ServiceContracts.Moderator;
using Feedbacks.Service.API.ServiceContracts.Participant;
using Feedbacks.Service.API.Services.Moderator;
using Feedbacks.Service.API.Services.Participant;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
