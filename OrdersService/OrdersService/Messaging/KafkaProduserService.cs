using OrdersService.Model;
using Confluent.Kafka;
using System.Text.Json;
using OrdersService.DTOs;

namespace OrdersService.Messaging
{
    public class KafkaProduserService : IKafkaProduserService
    {
        private readonly IProducer<string, string> _producer;
        public KafkaProduserService(IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092",
            };
            try
            {
                _producer = new ProducerBuilder<string, string>(config).Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании Kafka Producer: {ex.Message}");
                throw;
            }
        }
        public async Task SendMessageAsync(string topic, List<KafkaOrderItemDTO> kafkaOrderItemDTOs)
        {
            try
            {
                var jsonMessage = JsonSerializer.Serialize(kafkaOrderItemDTOs);
                Console.WriteLine($"Отправили сообщение: {jsonMessage}");
                await _producer.ProduceAsync(topic, new Message<string, string> { Key = kafkaOrderItemDTOs.First().OrderId.ToString(), Value = jsonMessage });
            }
            catch (ProduceException<string, string>)
            {
                Console.WriteLine("Не удалось отправить сообщение!");
            }
        }
    }
}
