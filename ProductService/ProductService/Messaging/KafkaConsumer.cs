using Confluent.Kafka;
using ProductsService.DTOs;
using System.Threading;
using System.Text.Json;
using ProductsService.Services;


namespace ProductsService.Messaging
{
    public class KafkaConsumer : IKafkaConsumer
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public KafkaConsumer(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                GroupId = configuration["Kafka:GroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
            _consumer = new ConsumerBuilder<string, string>(config).Build();
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task StartConsumingAsyng(CancellationToken cancellationToken)
        {
            await Task.Run(async() =>
            {
                var topics = new List<string>
                {
                    "orderIten-added"
                };
                _consumer.Subscribe(topics);
                while (!cancellationToken.IsCancellationRequested)
                {
                    var producer_message = _consumer.Consume(cancellationToken);
                    switch(producer_message.Topic)
                    {
                        case "orderIten-added":
                            var orderItem = JsonSerializer.Deserialize<OrderItemDTO>(producer_message.Message.Value);
                            if (orderItem != null)
                            {
                                await WorkWithProductInfoMessage(orderItem, cancellationToken);
                            }
                            break;
                    }
                }
            }, cancellationToken);
        }
        public async Task WorkWithProductInfoMessage(OrderItemDTO orderItemDTO, CancellationToken cancellationToken)
        {
            using(var scope = _serviceScopeFactory.CreateScope())
            {
                var orderItemSevice = scope.ServiceProvider.GetRequiredService<IProductService>();
                await orderItemSevice.PrintMessage(orderItemDTO);
            }
            
        }
    }
}
