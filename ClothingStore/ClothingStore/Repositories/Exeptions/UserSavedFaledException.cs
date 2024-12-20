namespace ClothingStore.Repositories.Exeptions
{
    public class UserSavedFaledException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Ошибка при сохранении данных о пользователе.";

        // Конструктор без параметров, использует сообщение по умолчанию
        public UserSavedFaledException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public UserSavedFaledException(string message) : base(message) { }
    }
}
