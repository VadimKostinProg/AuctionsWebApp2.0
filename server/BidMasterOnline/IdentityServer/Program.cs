using BidMasterOnline.Core;
using BidMasterOnline.Infrastructure;
using IdentityServer;
using IdentityServer.Services;
using IdentityServer.Services.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.ConfigureIdentityServer();

builder.Services.AddCoreServices()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IPasswordValidationService, PasswordValidationService>();
builder.Services.AddScoped<IUserManager, UserManager>();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();

app.UseAuthorization();

app.UseEndpoints(cfg =>
{
    cfg.MapRazorPages();
});

app.Run();
