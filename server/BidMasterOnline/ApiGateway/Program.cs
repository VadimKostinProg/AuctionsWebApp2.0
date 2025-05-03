using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOcelot();

var app = builder.Build();

app.UseEndpoints(options => options.MapControllers());

await app.UseOcelot();

app.Run();
