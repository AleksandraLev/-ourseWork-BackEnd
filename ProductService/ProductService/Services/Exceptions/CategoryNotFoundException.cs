namespace ProductsService.Services.Exceptions
{
    public class CategoryNotFoundException : Exception
    {
        // Сообщение-шаблон по умолчанию
        private const string DefaultMessageTemplate = "Категория \"{0}\" не найдена.";

        // Конструктор, принимающий параметр для включения в сообщение
        public CategoryNotFoundException(string categoty) : base(string.Format(DefaultMessageTemplate, categoty)) { }

        // Конструктор с внутренним исключением
        public CategoryNotFoundException(string categoty, Exception innerException) : base(string.Format(DefaultMessageTemplate, categoty), innerException) { }
    }
}
