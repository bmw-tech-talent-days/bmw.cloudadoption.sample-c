namespace BMW.CloudAdoption.Sample.Api.BackgroundServices;

public class SampleConsumerService : BackgroundService
{
    private readonly SampleConsumer _consumer;

    public SampleConsumerService(SampleConsumer consumer)
    {
        _consumer = consumer;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() => _consumer.Consume(stoppingToken), stoppingToken);
        return Task.CompletedTask;
    }
}