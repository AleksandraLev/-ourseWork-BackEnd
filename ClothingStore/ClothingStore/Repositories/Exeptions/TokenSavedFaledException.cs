namespace ClothingStore.Repositories.Exeptions
{
    public class TokenSavedFaledException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Ошибка при сохранении данных о токене.";

        // Конструктор без параметров, использует сообщение по умолчанию
        public TokenSavedFaledException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public TokenSavedFaledException(string message) : base(message) { }
    }
}
