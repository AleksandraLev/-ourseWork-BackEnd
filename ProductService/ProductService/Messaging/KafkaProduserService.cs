using Confluent.Kafka;
using ProductsService.DTOs;
using System.Text.Json;

namespace ProductsService.Messaging
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
            _producer = new ProducerBuilder<string, string>(config).Build();
        }
        public async Task SendMessageAsync(string topic, List<KafkaProductDTO> kafkaProductDTO)
        {
            try
            {
                var jsonMessage = JsonSerializer.Serialize(kafkaProductDTO);
                Console.WriteLine($"{jsonMessage}");
                await _producer.ProduceAsync(topic, new Message<string, string> { Key = kafkaProductDTO.First().OrderId.ToString(), Value = jsonMessage });
                Console.WriteLine("Отправляем обратно");
            }
            catch (ProduceException<string, string>)
            {
                Console.WriteLine("Не удалось отправить сообщение!");
            }
        }
    }
}
