namespace BMW.CloudAdoption.Sample.Api.Sample;

public class SampleProducer : KafkaProducer<string, string>
{
    public SampleProducer(IConfiguration configuration, ILogger<SampleProducer> logger) 
        : base(configuration, logger) { }

    protected override string GetTopic()
    {
        return _configuration.GetSection("KafkaTopics")
            .GetValue<string>("MyTopic") ?? string.Empty;
    }
    
    public void Produce(string id, SampleRequest? sample)
    {
        var json = sample is null 
            ? string.Empty 
            : JsonConvert.SerializeObject(sample, AppConstants.JsonSerializerSettings);
        
        Produce(id, json);
    }

    protected override void DeliveryHandler(DeliveryReport<string, string?> report)
    {
        _logger.LogInformation("Delivery report for sample {SampleNumber}. Status: {Status}", report.Key, report.Status);
    }
}