using Confluent.Kafka.Admin;

namespace BMW.CloudAdoption.Sample.Api.Kafka;

public class KafkaInitializer
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<KafkaInitializer> _logger;

    public KafkaInitializer(IConfiguration configuration, ILogger<KafkaInitializer> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        var config = new AdminClientConfig();
        _configuration.GetSection("Kafka").Bind(config);
        
        var topics = _configuration.GetSection("KafkaTopics").AsEnumerable();

        using IAdminClient? adminClient = new AdminClientBuilder(config).Build();
        var existingTopics = adminClient.GetMetadata(TimeSpan.FromSeconds(10)).Topics.Select(t => t.Topic).ToList();
        try
        {
            foreach (var topic in topics)
            {
                if (!string.IsNullOrEmpty(topic.Value) && !existingTopics.Contains(topic.Value))
                {
                    await adminClient.CreateTopicsAsync(new[]
                    {
                        new TopicSpecification { Name = topic.Value, ReplicationFactor = 3, NumPartitions = 1 }
                    });
                }
            }
        }
        catch (CreateTopicsException e)
        {
            _logger.LogError($"An error occured creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
        }
    }
}