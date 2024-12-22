using ProductsService.DTOs;


namespace ProductsService.Messaging
{
    public interface IKafkaConsumerService
    {
        Task StartConsumingAsyng(CancellationToken cancellationToken);
        Task WorkWithProductInfoMessage(List<KafkaOrderItemDTO> kafkaOrderItemDTO, CancellationToken cancellationToken);
    }
}
