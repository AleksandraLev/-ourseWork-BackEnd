namespace ClothingStore.Services.Exceptions
{
    public class GenerateAccessTokenException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Ошибка при создании Access Token.";

        // Конструктор без параметров, использует сообщение по умолчанию
        public GenerateAccessTokenException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public GenerateAccessTokenException(string message) : base(message) { }
    }
}
