namespace CrmManagementAPI.Interfaces
{
    public interface IEmailSender
    {
        
        bool IsConfigured { get; }

     
        Task SendAsync(string toEmail, string? toName, string subject, string body, string? ccEmail = null);
    }
}