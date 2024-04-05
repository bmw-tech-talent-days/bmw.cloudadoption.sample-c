using BMW.CloudAdoption.Sample.Api.Common;

namespace BMW.CloudAdoption.Sample.Api.Sample;

public class SampleProducer
{
    private readonly ILogger _logger;
    private readonly IProducer<string, string?> _producer;
    private readonly string _topic;
    
    public SampleProducer(ILogger<SampleProducer> logger, IConfiguration configuration)
    {
        _logger = logger;
        var config = new ProducerConfig();
        configuration.GetSection("Kafka").Bind(config);
        _producer = new ProducerBuilder<string, string?>(config).Build();
        _topic = configuration.GetSection("KafkaTopics").GetValue<string>("MyTopic") ?? string.Empty;
    }
    
    public void Produce(string id, SampleRequest? sample)
    {
        var json = sample is null 
            ? null 
            : JsonConvert.SerializeObject(sample, AppConstants.JsonSerializerSettings);
        
        _producer.Produce(_topic, new Message<string, string?> { Key = id, Value = json }, report =>
            _logger.LogInformation("Delivery report for sample {SampleNumber}. Status: {Status}", report.Key, report.Status));  
    }
}