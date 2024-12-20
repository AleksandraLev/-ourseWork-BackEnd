using ClothingStore.Model;

namespace ClothingStore.Messaging
{
    public interface IKafkaProduserService
    {
        Task SendMessageAsync(string topic, User user);
    }
}
