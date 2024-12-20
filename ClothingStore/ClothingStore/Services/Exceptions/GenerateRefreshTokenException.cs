namespace ClothingStore.Services.Exceptions
{
    public class GenerateRefreshTokenException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Ошибка при создании Refresh Token.";

        // Конструктор без параметров, использует сообщение по умолчанию
        public GenerateRefreshTokenException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public GenerateRefreshTokenException(string message) : base(message) { }
    }
}
