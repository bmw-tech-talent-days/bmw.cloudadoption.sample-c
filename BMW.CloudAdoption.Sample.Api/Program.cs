global using Microsoft.AspNetCore.Mvc;
global using Newtonsoft.Json;
global using Confluent.Kafka;

global using BMW.CloudAdoption.Sample.Api.BackgroundServices;
global using BMW.CloudAdoption.Sample.Api.Sample;
using BMW.CloudAdoption.Sample.Api.Common;

const string corsPolicy = "CorsPolicy";

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers()
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters = AppConstants.JsonSerializerSettings.Converters;
            options.SerializerSettings.ContractResolver = AppConstants.JsonSerializerSettings.ContractResolver;
        });
        
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddSwaggerGenNewtonsoftSupport();

    builder.Services.AddScoped<KafkaInitializer>();
    builder.Services.AddSingleton<SampleProducer>();
    builder.Services.AddSingleton<SampleConsumer>();
    builder.Services.AddSingleton<SampleCache>();

    builder.Services.AddHostedService<SampleConsumerService>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(corsPolicy, policyBuilder => policyBuilder
            .SetIsOriginAllowed((host) => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
        );
    });
}

async Task RunMigrationsAsync(WebApplication app)
{
    var kafkaInitializer = app.Services.CreateScope()
        .ServiceProvider.GetRequiredService<KafkaInitializer>();
    await kafkaInitializer.InitializeAsync();
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        await RunMigrationsAsync(app);
        app.UseSwagger();
        app.UseSwaggerUI();
    }
        
    app.UseCors(corsPolicy);
    app.UseHttpsRedirection();
    app.MapControllers();
    app.Run();
}
