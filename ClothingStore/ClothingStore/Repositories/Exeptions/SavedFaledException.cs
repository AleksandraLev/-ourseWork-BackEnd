namespace ClothingStore.Repositories.Exeptions
{
    public class SavedFaledException : Exception
    {
        // Сообщение по умолчанию
        private const string DefaultMessage = "Ошибка при сохранении данных.";

        // Конструктор без параметров, использует сообщение по умолчанию
        public SavedFaledException() : base(DefaultMessage) { }

        // Конструктор с возможностью передать собственное сообщение
        public SavedFaledException(string message) : base(message) { }
    }
}
