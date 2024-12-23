using Confluent.Kafka;
using ProductsService.DTOs;
using System.Threading;
using System.Text.Json;
using ProductsService.Services;
using ProductsService.Services.Exceptions;


namespace ProductsService.Messaging
{
    public class KafkaConsumerService : IKafkaConsumerService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IConsumer<string, string> _consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "ClothingStore-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
            _logger = logger;
            _consumer = new ConsumerBuilder<string, string>(config).Build();
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task StartConsumingAsyng(CancellationToken cancellationToken)
        {
            await Task.Run(async() =>
            {
                var topics = new List<string>
                {
                    "orderItems-added"
                };
                _consumer.Subscribe(topics);
                while (!cancellationToken.IsCancellationRequested)
                {
                    var producer_message = _consumer.Consume(cancellationToken);
                    switch(producer_message.Topic)
                    {
                        case "orderItems-added":
                            _logger.LogInformation("Получаем сообщение с topic \"orderItems-added\".");
                            var kafkaOrderItemDTO = JsonSerializer.Deserialize<List<KafkaOrderItemDTO>>(producer_message.Message.Value);
                            if (kafkaOrderItemDTO != null)
                            {
                                _logger.LogInformation("Список товаров в заказе:");
                                foreach (var item in kafkaOrderItemDTO)
                                {
                                    _logger.LogInformation($"OrderId: {item.OrderId}, ProductId: {item.ProductId}, Quantity: {item.Quantity}");
                                }
                                await WorkWithProductInfoMessage(kafkaOrderItemDTO, cancellationToken);
                            }
                            break;
                    }
                }
            }, cancellationToken);
        }
        public async Task WorkWithProductInfoMessage(List<KafkaOrderItemDTO> kafkaOrderItemDTO, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Обработка сообщения orderItems-added.");
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                    await productService.CheckOrderInfoAsync(kafkaOrderItemDTO);
                }
                _consumer.Commit();
            }
            catch (GetProductException)
            {
                _logger.LogError("Не удалось обработать сообщение orderIten-added.");
            }
            _logger.LogInformation("Сообщение orderItems-added обработано.");
        }
    }
}
