using Builder.Enums;
using Builder.Models;

namespace Builder;

internal class EmailBuilder
{
    private Guid _Id;
    private DateTime _SentOn;
    private EmailReason _Reason;
    private string? _To;
    private string? _Subject;
    private string? _HTMLBody;
    private IReadOnlyList<EmailAttachment>? _Attachments;


    public EmailBuilder SetId(Guid id)
    {
        _Id = id;
        return this;
    }
    public EmailBuilder SetSentOn(DateTime sentOn)
    {
        _SentOn = sentOn;
        return this;
    }
    public EmailBuilder SetReason(EmailReason reason)
    {
        _Reason = reason;
        return this;
    }
    public EmailBuilder SetTo(string to)
    {
        _To = to;
        return this;
    }
    public EmailBuilder SetSubject(string subject)
    {
        _Subject = subject;
        return this;
    }
    public EmailBuilder SetBody(string HTMLbody)
    {
        _HTMLBody = HTMLbody;
        return this;
    }
    public EmailBuilder SetAttachments(List<EmailAttachment> emailAttachments)
    {
        _Attachments = emailAttachments.AsReadOnly();
        return this;
    }

    public Email Build()
    {
        if (string.IsNullOrWhiteSpace(_To))
            throw new InvalidOperationException("To is required");

        if (string.IsNullOrWhiteSpace(_Subject))
            throw new InvalidOperationException("Subject is required");

        if (string.IsNullOrWhiteSpace(_HTMLBody))
            throw new InvalidOperationException("Body is required");

        return new Email
        {
            Id = _Id,
            SentOn = _SentOn,
            Reason = _Reason,
            To = _To,
            Subject = _Subject,
            HTMLBody = _HTMLBody,
            Attachments = _Attachments
        };
    }
}
