using InternetSentry.Services;
using InternetSentry.Services.Clients;
using InternetSentry.Workers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<TechniClient>();
builder.Services.AddSingleton<TechniService>();
builder.Services.AddSingleton<RozalinaClient>();
builder.Services.AddHostedService<InternetSentryWorker>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "InternetSentry API");
    options.RoutePrefix = string.Empty;
});

app.MapGet("/ping", () => "pong");
app.MapGet("/status", (TechniService techniService) => {
    return Results.Ok(techniService.CurrenStatus);
    });
app.MapGet("/pstatus", (TechniService techniService) => {
    return TypedResults.Ok(techniService.Pings);
});
app.Run();