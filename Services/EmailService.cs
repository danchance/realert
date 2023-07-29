using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace Realert.Services
{
    public class EmailService
    {

        private readonly IAmazonSimpleEmailService _amazonSimpleEmailService;

        public EmailService(IAmazonSimpleEmailService amazonSimpleEmailService) 
        {
            _amazonSimpleEmailService = amazonSimpleEmailService;
        }

        /*
         * 
         */
        public async Task<string> SendEmailAsync(List<string> toAdress, string bodyText, string subject, string senderAddress)
        {
            var messageId = "";
            try
            {
                var response = await _amazonSimpleEmailService.SendEmailAsync(
                    new SendEmailRequest
                    {
                        Destination = new Destination
                        {
                            ToAddresses = toAdress,
                        },
                        Message = new Message
                        {
                            Body = new Body
                            {
                                /*Html = new Content
                                {
                                    Charset = "UTF-8",
                                    Data = bodyHtml
                                },*/
                                Text = new Content
                                {
                                    Charset = "UTF-8",
                                    Data = bodyText
                                }
                            },
                            Subject = new Content
                            {
                                Charset = "UTF-8",
                                Data = subject
                            }
                        },
                        Source = senderAddress
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
