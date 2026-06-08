using Microsoft.AspNetCore.Identity.UI.Services;

namespace BG3BuildPlanner.Data;

public sealed class AppNoOpEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Task.CompletedTask;
    }
}
