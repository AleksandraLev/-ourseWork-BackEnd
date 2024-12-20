namespace ProductsService.Services.Exceptions
{
    public class ProductNotFoundException : Exception
    {
        // Сообщение-шаблон по умолчанию
        private const string DefaultMessageTemplate = "Товар \"{0}\" не найден.";

        // Конструктор, принимающий параметр для включения в сообщение
        public ProductNotFoundException(string product) : base(string.Format(DefaultMessageTemplate, product)) { }

        // Конструктор с внутренним исключением
        public ProductNotFoundException(string product, Exception innerException) : base(string.Format(DefaultMessageTemplate, product), innerException) { }
    }
}
