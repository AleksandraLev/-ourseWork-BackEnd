using ClothingStore.Model;

namespace ClothingStore.Services.Exceptions
{
    public class UserNotFoundException : Exception
    {
        // Сообщение-шаблон по умолчанию
        private const string DefaultMessageTemplate = "Пользователь с номером телефона \"{0}\" не найден.";
        
        // Конструктор, принимающий параметр для включения в сообщение
        public UserNotFoundException(string phoneNumber) : base(string.Format(DefaultMessageTemplate, phoneNumber)) { }
        
        // Конструктор с внутренним исключением
        public UserNotFoundException(string phoneNumber, Exception innerException) : base(string.Format(DefaultMessageTemplate, phoneNumber), innerException) { }
    }
}
