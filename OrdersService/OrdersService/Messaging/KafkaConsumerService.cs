using Confluent.Kafka;
using OrdersService.DTOs;
using OrdersService.Model;
using OrdersService.Repositories.Exceptions;
using OrdersService.Services;
using OrdersService.Services.Exceptions;
using System.Text.Json;

namespace OrdersService.Messaging
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
            await Task.Run(async () =>
            {
                var topics = new List<string>
                {
                    "product-change"
                };
                _consumer.Subscribe(topics);
                while (!cancellationToken.IsCancellationRequested)
                {
                    var producer_message = _consumer.Consume(cancellationToken);
                    switch (producer_message.Topic)
                    {
                        case "product-change":
                            var kafkaProductDTO = JsonSerializer.Deserialize<List<KafkaProductDTO>>(producer_message.Message.Value);
                            if (kafkaProductDTO != null)
                            {
                                _logger.LogInformation("Список товаров в заказе:");
                                foreach (var item in kafkaProductDTO)
                                {
                                    _logger.LogInformation($"OrderId: {item.OrderId}, ProductId: {item.ProductId}, Quantity: {item.Quantity}");
                                }
                                await WorkWithOrderInfoMessageAsync(kafkaProductDTO, cancellationToken);
                            }
                            break;
                    }
                }
            }, cancellationToken);
        }

        public async Task WorkWithOrderInfoMessageAsync(List<KafkaProductDTO> kafkaProductDTO, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Обработка сообщения product-change");
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    
                    if (kafkaProductDTO.All(item => item.ProductExist && item.QuantityExist))
                    {
                        var orderItemService = scope.ServiceProvider.GetRequiredService<IOrderItemService>();
                        await orderItemService.WorkWithProductInfoAsync(kafkaProductDTO);
                        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                        await orderService.ShipOrderAsync(kafkaProductDTO.First().OrderId);
                    }
                    else
                    {
                        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                        await orderService.AfterProcessOrderAsync(kafkaProductDTO);
                    }
                }
                _consumer.Commit();
            }
            catch (OrderSavedFailedException)
            {
                _logger.LogError("Не удалось обработать сообщение product-change.");
            }
            catch (ChangeOrderStatusException)
            {
                _logger.LogError("Не удалось обработать сообщение product-change.");
            }
            catch (OrderItemSavedFailedException)
            {
                _logger.LogError("Не удалось обработать сообщение product-change.");
            }
            _logger.LogInformation("Сообщение product-change обработано.");
        }
    }
}
