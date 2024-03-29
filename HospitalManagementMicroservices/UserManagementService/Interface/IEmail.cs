namespace UserManagementService.Interface
{
    public interface IEmail
    {
        Task<bool> SendEmailAsync(string to, string subject, string htmlMessage);

    }
}
