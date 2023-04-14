namespace BMW.CloudAdoption.Sample.Api.Kafka;

// ReSharper disable InconsistentNaming

public static class KafkaExtensions
{
    public static LogLevel ToLogLevel(this SyslogLevel logLevel)
    {
        return logLevel switch  
        {  
            SyslogLevel.Critical => LogLevel.Critical,  
            SyslogLevel.Emergency or SyslogLevel.Alert or SyslogLevel.Error => LogLevel.Error,  
            SyslogLevel.Warning => LogLevel.Warning,
            SyslogLevel.Info or SyslogLevel.Notice  => LogLevel.Information,  
            SyslogLevel.Debug => LogLevel.Debug,  
            _ => LogLevel.Trace,
        };
    }
}