namespace OrdersService.Services.Exceptions
{
    public class SelectOrderException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Ошибка при получении информации о заказе!";

        // Конструктор без параметров, использует сообщение по умолчанию
        public SelectOrderException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public SelectOrderException(string message) : base(message) { }
    }
}
