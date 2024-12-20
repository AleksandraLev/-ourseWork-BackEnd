namespace ProductsService.Services.Exceptions
{
    public class CategoryIsNullException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Ошибка при поиске категорий!";

        // Конструктор без параметров, использует сообщение по умолчанию
        public CategoryIsNullException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public CategoryIsNullException(string message) : base(message) { }
    }
}
