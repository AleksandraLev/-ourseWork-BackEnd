
using Confluent.Kafka;

namespace ProductsService.Messaging
{
    public class KafkaConsumerBackgroundService : BackgroundService
    {
        private readonly IKafkaConsumerService _consumerService;
        public KafkaConsumerBackgroundService(IKafkaConsumerService consumerService)
        {
            _consumerService = consumerService;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumerService.StartConsumingAsyng(stoppingToken);
        }
    }
}
