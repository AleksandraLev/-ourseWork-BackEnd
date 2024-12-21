using ProductsService.DTOs;


namespace ProductsService.Messaging
{
    public interface IKafkaConsumer
    {
        Task StartConsumingAsyng(CancellationToken cancellationToken);
        Task WorkWithProductInfoMessage(OrderItemDTO orderItemDTO, CancellationToken cancellationToken);
    }
}
