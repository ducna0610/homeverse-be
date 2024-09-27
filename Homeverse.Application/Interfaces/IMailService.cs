namespace Homeverse.Application.Interfaces;

public interface IMailService
{
    public Task SendAsync(string email, string subject, string messageBody);
}
