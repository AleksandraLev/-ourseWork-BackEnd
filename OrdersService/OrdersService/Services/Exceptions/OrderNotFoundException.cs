namespace OrdersService.Services.Exceptions
{
    public class OrderNotFoundException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Заказ не найден!";

        // Конструктор без параметров, использует сообщение по умолчанию
        public OrderNotFoundException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public OrderNotFoundException(string message) : base(message) { }
    }
}
