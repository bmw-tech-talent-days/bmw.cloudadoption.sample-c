using BMW.CloudAdoption.Sample.Api.Common;

namespace BMW.CloudAdoption.Sample.Api.Sample;

public class SampleConsumer
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public SampleConsumer(
        IConfiguration configuration,
        ILogger<SampleConsumer> logger,
        IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public void Consume(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig();
        _configuration.GetSection("Kafka").Bind(config);
        var consumer = new ConsumerBuilder<string, string?>(config).Build();

        var topic = _configuration.GetSection("KafkaTopics").GetValue<string>("MyTopic") ?? string.Empty;
        consumer.Subscribe(topic);

        _logger.LogInformation("Kafka consumer on topic [{Topic}] started", topic);
        while (!stoppingToken.IsCancellationRequested)
        {
            var consumeResult = consumer.Consume(stoppingToken);
            
            using var scope = _serviceProvider.CreateScope();
            var sampleCache = scope.ServiceProvider.GetRequiredService<SampleCache>();

            var sampleRequest = JsonConvert.DeserializeObject<SampleRequest>(
                consumeResult.Message.Value ?? string.Empty,
                AppConstants.JsonSerializerSettings);
            if (sampleRequest is not null)
            {
                sampleCache.AddOrUpdate(sampleRequest);
            }
        }

        consumer.Close();
        _logger.LogInformation("Kafka consumer on topic [{Topic}] Stopped", topic);
    }
}