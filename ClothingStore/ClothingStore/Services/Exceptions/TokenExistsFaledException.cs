namespace ClothingStore.Services.Exceptions
{
    public class TokenExistsFaledException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Такой токен не найден.";

        // Конструктор без параметров, использует сообщение по умолчанию
        public TokenExistsFaledException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public TokenExistsFaledException(string message) : base(message) { }
    }
}
