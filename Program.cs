using InternetSentry.Services;
using InternetSentry.Services.Clients;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{

});
builder.Services.AddSingleton<TechniClient>();
builder.Services.AddHostedService<TechniService>();

var app = builder.Build();