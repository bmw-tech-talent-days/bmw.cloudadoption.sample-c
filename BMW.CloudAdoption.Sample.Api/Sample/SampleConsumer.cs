namespace BMW.CloudAdoption.Sample.Api.Sample;

public class SampleConsumer : KafkaConsumer<string, string>
{
    private readonly IServiceProvider _serviceProvider;
    
    public SampleConsumer(IServiceProvider serviceProvider,
        IConfiguration configuration, ILogger<SampleConsumer> logger) 
        : base(configuration, logger)
    {
        _serviceProvider = serviceProvider;
    }
    
    protected override string GetTopic()
    {
        return _configuration.GetSection("KafkaTopics")
            .GetValue<string>("MyTopic") ?? string.Empty;
    }

    protected override Task ProcessConsumeResultAsync(ConsumeResult<string, string?> consumeResult)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var sampleCache = scope.ServiceProvider.GetRequiredService<SampleCache>();

            var sampleRequest = JsonConvert.DeserializeObject<SampleRequest>(consumeResult.Message.Value ?? string.Empty,
                AppConstants.JsonSerializerSettings);
            if (sampleRequest is not null)
            {
                sampleCache.AddOrUpdate(sampleRequest);
            }
        }
        catch (Exception e)
        {
            // Handle Exception
            _logger.LogWarning(e, "Could not process SampleRequest Message {MessageKey}", consumeResult.Message.Key);
        }

        return Task.CompletedTask;
    }
}