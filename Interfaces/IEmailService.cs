namespace Realert.Interfaces
{
    public interface IEmailService
    {
        Task<string> SendEmailAsync(List<string> toAddress, string bodyHtml, string subject);
    }
}
