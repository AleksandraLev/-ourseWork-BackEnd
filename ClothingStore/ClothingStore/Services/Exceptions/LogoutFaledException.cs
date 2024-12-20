namespace ClothingStore.Services.Exceptions
{
    public class LogoutFaledException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Ошибка при попытке выхода из аккаунта.";

        // Конструктор без параметров, использует сообщение по умолчанию
        public LogoutFaledException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public LogoutFaledException(string message) : base(message) { }
    }
}
