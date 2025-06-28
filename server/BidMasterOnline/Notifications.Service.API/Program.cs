using Notifications.Service.API.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<EmailSender>();
builder.Services.AddSingleton<ConsumerStartupService>();


// Configuring Brevo.API
sib_api_v3_sdk.Client.Configuration.Default.ApiKey.Add("api-key", builder.Configuration["BrevoSettings:ApiKey"]);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

await StartConsumer(app);

app.Run();

async Task StartConsumer(WebApplication app)
{
    ConsumerStartupService consumerStartupService = app.Services.GetRequiredService<ConsumerStartupService>();

    await consumerStartupService.StartConsumerAsync();
}