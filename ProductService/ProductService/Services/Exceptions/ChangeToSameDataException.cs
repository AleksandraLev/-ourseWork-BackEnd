namespace ProductsService.Services.Exceptions
{
    public class ChangeToSameDataException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Ошибка при изменении данных. Попытка изменить данные на те же самые значения.";

        // Конструктор без параметров, использует сообщение по умолчанию
        public ChangeToSameDataException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public ChangeToSameDataException(string message) : base(message) { }
    }
}
