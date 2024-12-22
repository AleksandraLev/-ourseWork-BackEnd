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
        private readonly IConsumer<string, string> _consumer;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public KafkaConsumerService(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "ClothingStore-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };
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
                                Console.WriteLine("Список товаров в заказе:");
                                foreach (var item in kafkaProductDTO)
                                {
                                    Console.WriteLine($"OrderId: {item.OrderId}, ProductId: {item.ProductId}, Quantity: {item.Quantity}");
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
                Console.WriteLine("Не удалось обработать сообщение product-change.");
            }
            catch (ChangeOrderStatusException)
            {
                Console.WriteLine("Не удалось обработать сообщение product-change.");
            }
            catch (OrderItemSavedFailedException)
            {
                Console.WriteLine("Не удалось обработать сообщение product-change.");
            }
        }
    }
}
