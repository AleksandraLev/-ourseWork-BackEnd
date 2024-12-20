namespace ClothingStore.Repositories.Exeptions
{
    public class PhoneNumberExistsException : Exception
    {
        public PhoneNumberExistsException(string message) : base(message) { }
    }
}
