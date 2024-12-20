using OrdersService.Model;
using Confluent.Kafka;
using System.Text.Json;

namespace OrdersService.Messaging
{
    public class KafkaProduserService : IKafkaProduserService
    {
        private readonly IProducer<string, string> _producer;
        public KafkaProduserService(IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka: BootstrapServers"]
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }
        public async Task SendMessageAsync(string topic, OrderItem orderItem)
        {
            try
            {
                var jsonMessage = JsonSerializer.Serialize(orderItem);
                await _producer.ProduceAsync(topic, new Message<string, string> { Key = orderItem.Id.ToString(), Value = jsonMessage });
            }
            catch (ProduceException<string, string>)
            {
                Console.WriteLine("Не удалось отправить сообщение!");
            }
        }
    }
}
