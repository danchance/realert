using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace Realert.Services
{
    public class EmailService
    {

        private readonly IAmazonSimpleEmailService _amazonSimpleEmailService;
        private const string _senderAddress = "dan.chance@outlook.com";

        public EmailService(IAmazonSimpleEmailService amazonSimpleEmailService) 
        {
            _amazonSimpleEmailService = amazonSimpleEmailService;
        }

        /*
         * Uses AWS SES to send an email.
         */
        public async Task<string> SendEmailAsync(List<string> toAddress, string bodyHtml, string subject)
        {
            var messageId = "";
            try
            {
                var response = await _amazonSimpleEmailService.SendEmailAsync(
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
                                    Data = bodyHtml
                                }
                            },
                            Subject = new Content
                            {
                                Charset = "UTF-8",
                                Data = subject
                            }
                        },
                        Source = _senderAddress
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
