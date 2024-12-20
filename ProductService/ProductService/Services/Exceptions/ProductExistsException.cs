namespace ProductsService.Services.Exceptions
{
    public class ProductExistsException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Такой товар уже существует.";

        // Конструктор без параметров, использует сообщение по умолчанию
        public ProductExistsException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public ProductExistsException(string message) : base(message) { }
    }
}
