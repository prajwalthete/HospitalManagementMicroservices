namespace UserManagementService.GlobleExceptionhandler
{
    public class InvalidPasswordException : Exception
    {
        public InvalidPasswordException(string message) : base(message) { }
    }
}
