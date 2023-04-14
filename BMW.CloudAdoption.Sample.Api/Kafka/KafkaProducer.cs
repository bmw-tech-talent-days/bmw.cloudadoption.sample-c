namespace BMW.CloudAdoption.Sample.Api.Kafka;

// ReSharper disable InconsistentNaming
// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable MemberCanBePrivate.Global

public abstract class KafkaProducer<TKey, TValue> : IDisposable 
{
    protected readonly string Topic;
    protected readonly ILogger _logger;
    protected readonly IConfiguration _configuration;
    
    private readonly IProducer<TKey, TValue?> _producer;
    
    protected KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer<TKey, TValue>> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        var config = new ProducerConfig();
        _configuration.GetSection("Kafka").Bind(config);
        Configure(config);
        _producer = BuildProducer(config);

        Topic = GetTopic();
        if (string.IsNullOrEmpty(Topic))
            throw new ArgumentException("Producer Topic Required", nameof(Topic));
    }

    protected virtual void Configure(ProducerConfig config) { }
    protected virtual IProducer<TKey, TValue?> BuildProducer(ProducerConfig config)
    {
        return new ProducerBuilder<TKey, TValue?>(config)
            .SetErrorHandler((_, error) => _logger.LogError("{Message}", error.Reason))
            .SetLogHandler((_, msg) => _logger.Log(msg.Level.ToLogLevel(), "{Message}", msg.Message))
            .Build();
    }
    
    protected abstract string GetTopic();
    
    public void Produce(TKey key, TValue? value = default, Headers? headers = null)
    {
        _producer.Produce(Topic, new Message<TKey, TValue?> { Key = key, Value = value, Headers = headers}, DeliveryHandler);  
    }

    protected virtual void DeliveryHandler(DeliveryReport<TKey, TValue?> report)
    {
        _logger.LogDebug("Delivery report for {Key}. Status: {Status}", report.Key, report.Status);
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();  
    } 
}