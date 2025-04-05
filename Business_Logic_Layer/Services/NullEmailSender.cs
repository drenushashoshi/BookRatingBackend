using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Identity;

namespace FBookRating.Services
{
    public class NullEmailSender : IEmailSender<ApplicationUser>
    {
        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            return Task.CompletedTask;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Do nothing or log that an email was "sent" for testing purposes.
            return Task.CompletedTask;
        }

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            return Task.CompletedTask;
        }

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            return Task.CompletedTask;
        }
    }
}
