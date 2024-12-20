namespace ClothingStore.Services.Exceptions
{
    public class InvalidPasswordException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Неверный пароль!";

        // Конструктор без параметров, использует сообщение по умолчанию
        public InvalidPasswordException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public InvalidPasswordException(string message) : base(message) { }
    }
}
