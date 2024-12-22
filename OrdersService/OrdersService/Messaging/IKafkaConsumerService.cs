using OrdersService.DTOs;

namespace OrdersService.Messaging
{
    public interface IKafkaConsumerService
    {
        Task StartConsumingAsyng(CancellationToken cancellationToken);
        Task WorkWithOrderInfoMessageAsync(List<KafkaProductDTO> kafkaProductDTO, CancellationToken cancellationToken);
    }
}
