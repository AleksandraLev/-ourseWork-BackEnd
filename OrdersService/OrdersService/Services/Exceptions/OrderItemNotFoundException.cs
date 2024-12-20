namespace OrdersService.Services.Exceptions
{
    public class OrderItemNotFoundException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Часть заказа не найдена!";

        // Конструктор без параметров, использует сообщение по умолчанию
        public OrderItemNotFoundException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public OrderItemNotFoundException(string message) : base(message) { }
    }
}
