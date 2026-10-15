using Builder.Enums;
using Builder.Models;

namespace Builder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Builder Pattern");

            EmailBuilder emailBuilder = new();

            Email email = emailBuilder
                                .SetId(Guid.NewGuid())
                                .SetSentOn(DateTime.Now)
                                .SetReason(EmailReason.Welcome)
                                .SetTo("recipient@example.com")
                                .SetSubject("Builder Pattern")
                                .SetBody("<p>This is an example of the Builder Pattern.</p>")
                                .SetAttachments([
                                    new() { FileName = "attachment1.txt", Content = [0x01, 0x02, 0x03] },
                                    new() { FileName = "attachment2.txt", Content = [0x04, 0x05, 0x06] }
                                ])
                                .Build();


            Console.WriteLine($"ID: {email.Id}");
            Console.WriteLine($"Sent On: {email.SentOn}");
            Console.WriteLine($"Reason: {email.Reason}");

            Console.WriteLine($"To: {email.To}");
            Console.WriteLine($"Subject: {email.Subject}");
            Console.WriteLine($"HTML Body: {email.HTMLBody}");

            Console.WriteLine($"Attachments: {email.Attachments?.Count}");
        }
    }
}
