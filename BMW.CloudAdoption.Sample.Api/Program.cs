global using Microsoft.AspNetCore.Mvc;
global using Newtonsoft.Json;
global using Confluent.Kafka;

global using BMW.CloudAdoption.Sample.Api;
global using BMW.CloudAdoption.Sample.Api.BackgroundServices;
global using BMW.CloudAdoption.Sample.Api.Kafka;
global using BMW.CloudAdoption.Sample.Api.Sample;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();
await app.RunMigrationsAsync();
app.ConfigureApp();

app.MapGet("/", () => Results.Ok());
app.Run();
