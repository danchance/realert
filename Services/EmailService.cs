using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Realert.Interfaces;

namespace Realert.Services
{
    public sealed class EmailService : IEmailService
    {
        // Email address alerts are sent from.
        private const string SenderAddress = "dan.chance@outlook.com";

        // Fields.
        private readonly IAmazonSimpleEmailService amazonSimpleEmailService;

        public EmailService(IAmazonSimpleEmailService amazonSimpleEmailService)
        {
            this.amazonSimpleEmailService = amazonSimpleEmailService;
        }

        /// <summary>
        /// Method uses AWS Simple Email Service to send an email.
        /// </summary>
        /// <param name="toAddress">Address to send the email to.</param>
        /// <param name="bodyHtml">HTML body of the email message.</param>
        /// <param name="subject">Subject of the email.</param>
        /// <returns>Email message Id.</returns>
        public async Task<string> SendEmailAsync(List<string> toAddress, string bodyHtml, string subject)
        {
            var messageId = "";
            try
            {
                var response = await this.amazonSimpleEmailService.SendEmailAsync(
                    new SendEmailRequest
                    {
                        Destination = new Destination
                        {
                            ToAddresses = toAddress,
                        },
                        Message = new Message
                        {
                            Body = new Body
                            {
                                Html = new Content
                                {
                                    Charset = "UTF-8",
                                    Data = bodyHtml,
                                },
                            },
                            Subject = new Content
                            {
                                Charset = "UTF-8",
                                Data = subject,
                            },
                        },
                        Source = SenderAddress,
                    });
                messageId = response.MessageId;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendEmailAsync failed with exception: " + ex.Message);
            }

            return messageId;
        }
    }
}
