namespace ProductsService.Repositories.Exceptions
{
    public class CategorySavedFaledException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Ошибка при сохранении данных о категории.";

        // Конструктор без параметров, использует сообщение по умолчанию
        public CategorySavedFaledException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public CategorySavedFaledException(string message) : base(message) { }
    }
}
