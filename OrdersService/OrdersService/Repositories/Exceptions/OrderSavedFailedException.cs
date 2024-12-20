namespace OrdersService.Repositories.Exceptions
{
    public class OrderSavedFailedException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Ошибка при сохранении данных о заказе.";

        // Конструктор без параметров, использует сообщение по умолчанию
        public OrderSavedFailedException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public OrderSavedFailedException(string message) : base(message) { }
    }
}
