using Builder.Enums;

namespace Builder.Models;

internal class Email
{
    public Guid Id { get; init; }
    public DateTime SentOn { get; init; }
    public EmailReason Reason { get; init; }

    public string To { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string HTMLBody { get; init; } = string.Empty;

    public IReadOnlyList<EmailAttachment>? Attachments { get; init; }
}
