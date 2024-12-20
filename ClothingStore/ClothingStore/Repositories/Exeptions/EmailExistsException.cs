namespace ClothingStore.Repositories.Exeptions
{
    public class EmailExistsException : Exception
    {
        public EmailExistsException(string message) : base(message) { }
    }
}
