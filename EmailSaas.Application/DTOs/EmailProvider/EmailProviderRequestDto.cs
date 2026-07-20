namespace EmailSaas.Application.DTOs.EmailProvider;

public class EmailProviderRequestDto
{
    public int ClientID { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string? ReplyToEmail { get; set; }
    public string? SMTPHost { get; set; }
    public int? SMTPPort { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? ApiKey { get; set; }
    public string? APIEndPoint { get; set; }
    public bool IsDefault { get; set; }
    public string CreatedBy { get; set; } = string.Empty;

    // IMAP bounce mailbox fields
    public string? IMAPHost { get; set; }
    public int? IMAPPort { get; set; }
    public string? IMPAUserName { get; set; }
    public string? IMAPPassword { get; set; }
    public bool? IMAPSSL { get; set; }
//    public bool? BounceMonitoringEnabled { get; set; }
}