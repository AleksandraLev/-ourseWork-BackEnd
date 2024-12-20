using OrdersService.Model;

namespace OrdersService.Messaging
{
    public interface IKafkaProduserService
    {
        Task SendMessageAsync(string topic, OrderItem oderItem);
    }
}
