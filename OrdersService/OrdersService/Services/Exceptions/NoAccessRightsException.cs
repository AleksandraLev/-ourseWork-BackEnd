namespace OrdersService.Services.Exceptions
{
    public class NoAccessRightsException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Нет доступа к запрашиваемому действию!";

        // Конструктор без параметров, использует сообщение по умолчанию
        public NoAccessRightsException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public NoAccessRightsException(string message) : base(message) { }
    }
}
