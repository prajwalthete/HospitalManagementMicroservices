namespace UserManagementService.GlobleExceptionhandler
{
    public class InvalidEmailFormatException : Exception
    {
        public InvalidEmailFormatException(string message) : base(message) { }
    }
}
