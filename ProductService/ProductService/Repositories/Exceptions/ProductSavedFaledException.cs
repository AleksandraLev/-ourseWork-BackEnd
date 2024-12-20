namespace ProductsService.Repositories.Exceptions
{
    public class ProductSavedFaledException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Ошибка при сохранении данных о товаре.";

        // Конструктор без параметров, использует сообщение по умолчанию
        public ProductSavedFaledException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public ProductSavedFaledException(string message) : base(message) { }
    }
}
