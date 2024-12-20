namespace OrdersService.Repositories.Exceptions
{
    public class OrderItemSavedFailedException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Ошибка при сохранении данных о части заказа.";

        // Конструктор без параметров, использует сообщение по умолчанию
        public OrderItemSavedFailedException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public OrderItemSavedFailedException(string message) : base(message) { }
    }
}
