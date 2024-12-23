using Confluent.Kafka;
using ProductsService.DTOs;
using ProductsService.Services;
using System.Text.Json;

namespace ProductsService.Messaging
{
    public class KafkaProduserService : IKafkaProduserService
    {
        private readonly ILogger<KafkaProduserService> _logger;
        private readonly IProducer<string, string> _producer;
        public KafkaProduserService(ILogger<KafkaProduserService> logger, IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
            };
            _logger = logger;
            _producer = new ProducerBuilder<string, string>(config).Build();
        }
        public async Task SendMessageAsync(string topic, List<KafkaProductDTO> kafkaProductDTO)
        {
            try
            {
                var jsonMessage = JsonSerializer.Serialize(kafkaProductDTO);
                _logger.LogInformation($"Создали json-сообщение для отправки: {jsonMessage}");
                await _producer.ProduceAsync(topic, new Message<string, string> { Key = kafkaProductDTO.First().OrderId.ToString(), Value = jsonMessage });
                _logger.LogInformation("Отправляем сообщение.");
            }
            catch (ProduceException<string, string>)
            {
                _logger.LogError("Не удалось отправить сообщение!");
            }
            _logger.LogInformation("Сообщение отправленно.");
        }
    }
}
