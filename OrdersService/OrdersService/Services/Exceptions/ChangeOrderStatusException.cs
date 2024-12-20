namespace OrdersService.Services.Exceptions
{
    public class ChangeOrderStatusException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Ошибка при изменении статуса заказа!";

        // Конструктор без параметров, использует сообщение по умолчанию
        public ChangeOrderStatusException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public ChangeOrderStatusException(string message) : base(message) { }
    }
}
