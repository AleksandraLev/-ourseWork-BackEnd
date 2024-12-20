namespace ProductsService.Services.Exceptions
{
    public class CategoryExistsException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Такая категория уже существует.";

        // Конструктор без параметров, использует сообщение по умолчанию
        public CategoryExistsException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public CategoryExistsException(string message) : base(message) { }
    }
}
