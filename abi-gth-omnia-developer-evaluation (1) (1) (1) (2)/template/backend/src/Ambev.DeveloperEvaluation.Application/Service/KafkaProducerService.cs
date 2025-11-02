using Ambev.DeveloperEvaluation.Common.Config;
using Confluent.Kafka;

namespace Ambev.DeveloperEvaluation.Application.Service;

public class KafkaProducerService<TConfig> : IDisposable where TConfig : KafkaConfig
{
    private readonly IProducer<string, string> _producer;
    private readonly TConfig _config;

    public KafkaProducerService(string bootstrapServers, TConfig config)
    {
        _config = config;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
    }

    public async Task PublicarAsync(string content, string keyRecived = null)
    {
        var message = new Message<string, string>
        {
            Key = keyRecived,
            Value = content
        };

        try
        {
            var result = await _producer.ProduceAsync(_config.TopicName, message);
            Console.WriteLine($"Message published to {result.TopicPartitionOffset}");
        }
        catch (ProduceException<string, string> ex)
        {
            Console.WriteLine($"Error publishing message: {ex.Error.Reason}");
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}