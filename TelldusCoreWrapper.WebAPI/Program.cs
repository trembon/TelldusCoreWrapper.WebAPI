using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TelldusCoreWrapper;
using TelldusCoreWrapper.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(options =>
{
    options.AddConfiguration(builder.Configuration.GetSection("Logging"));
    options.AddConsole();
});

builder.Services.AddSingleton<IWebhookService, WebhookService>();
builder.Services.AddSingleton<ITelldusCoreService, TelldusCoreService>();
builder.Services.AddSingleton<ITelldusEventService, TelldusEventService>();
builder.Services.AddSingleton<ITelldusCommandService, TelldusCommandService>();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Services.GetService<ITelldusCoreService>().InitializeInThread(3000);
app.Services.GetService<ITelldusEventService>().Initialize();

app.MapControllers();

app.Run();
