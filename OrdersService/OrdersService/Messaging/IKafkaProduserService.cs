using OrdersService.DTOs;
using OrdersService.Model;

namespace OrdersService.Messaging
{
    public interface IKafkaProduserService
    {
        //Task SendMessageAsync(string topic, KafkaOrderItemDTO kafkaOrderItemDTO);
        Task SendMessageAsync(string topic, List<KafkaOrderItemDTO> kafkaOrderItemDTOs);
    }
}
