using ProductsService.DTOs;

namespace ProductsService.Messaging
{
    public interface IKafkaProduserService
    {
        Task SendMessageAsync(string topic, List<KafkaProductDTO> kafkaProductDTO);
    }
}
