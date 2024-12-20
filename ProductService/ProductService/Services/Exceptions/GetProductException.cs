namespace ProductsService.Services.Exceptions
{
    public class GetProductException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Ошибка при полученнии товара/товаров!";

        // Конструктор без параметров, использует сообщение по умолчанию
        public GetProductException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public GetProductException(string message) : base(message) { }
    }
}
