namespace BMW.CloudAdoption.Sample.Api.Kafka;

// ReSharper disable InconsistentNaming
// ReSharper disable VirtualMemberCallInConstructor
// ReSharper disable MemberCanBePrivate.Global

public abstract class KafkaConsumer<TKey, TValue> : IDisposable 
{
    protected readonly string Topic;
    protected readonly ILogger _logger;
    protected readonly IConfiguration _configuration;
    
    protected readonly IConsumer<TKey, TValue?> _consumer;
    
    protected KafkaConsumer(IConfiguration configuration, ILogger<KafkaConsumer<TKey, TValue>> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        var config = new ConsumerConfig();
        _configuration.GetSection("Kafka").Bind(config);
        Configure(config);
        _consumer = BuildConsumer(config);
        
        Topic = GetTopic();
        if (string.IsNullOrEmpty(Topic))
            throw new ArgumentException("Consumer Topic Required", nameof(Topic));
        
        _consumer.Subscribe(Topic);
    }

    protected virtual void Configure(ConsumerConfig config) { }
    protected virtual IConsumer<TKey, TValue?> BuildConsumer(ConsumerConfig config)
    {
        return new ConsumerBuilder<TKey, TValue?>(config)
            .SetErrorHandler((_, error) => _logger.LogError("{Message}", error.Reason))
            .SetLogHandler((_, msg) => _logger.Log(msg.Level.ToLogLevel(), "{Message}", msg.Message))
            .Build();
    }
    
    protected abstract string GetTopic();
    protected abstract Task ProcessConsumeResultAsync(ConsumeResult<TKey, TValue?> consumeResult);

    public async Task ConsumeAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer on topic [{topic}] started", Topic);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                if (consumeResult.IsPartitionEOF)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
                    continue;
                }
                
                await ProcessConsumeResultAsync(consumeResult);
            }
            catch (OperationCanceledException)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        _logger.LogInformation("Kafka consumer on topic [{topic}] Stopped", Topic);
    }

    public void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
    }
}