namespace OrdersService.Services.Exceptions
{
    public class SelectOrderItemException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Ошибка при получении информации о части заказа!";

        // Конструктор без параметров, использует сообщение по умолчанию
        public SelectOrderItemException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public SelectOrderItemException(string message) : base(message) { }
    }
}
