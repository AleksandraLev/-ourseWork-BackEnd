using OrdersService.Model;
using Confluent.Kafka;
using System.Text.Json;
using OrdersService.DTOs;
using OrdersService.Services;

namespace OrdersService.Messaging
{
    public class KafkaProduserService : IKafkaProduserService
    {
        private readonly ILogger<KafkaProduserService> _logger;
        private readonly IProducer<string, string> _producer;
        public KafkaProduserService(ILogger<KafkaProduserService> logger, IConfiguration configuration)
        {
            _logger = logger;
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
            };
            try
            {
                _logger.LogInformation("Создание Kafka Producer.");
                _producer = new ProducerBuilder<string, string>(config).Build();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при создании Kafka Producer: {ex.Message}");
                throw;
            }
            _logger.LogInformation("Kafka Producer создан.");
        }
        public async Task SendMessageAsync(string topic, List<KafkaOrderItemDTO> kafkaOrderItemDTOs)
        {
            try
            {
                var jsonMessage = JsonSerializer.Serialize(kafkaOrderItemDTOs);
                _logger.LogInformation($"Создали json-сообщение для отправки: {jsonMessage}");
                await _producer.ProduceAsync(topic, new Message<string, string> { Key = kafkaOrderItemDTOs.First().OrderId.ToString(), Value = jsonMessage });
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
