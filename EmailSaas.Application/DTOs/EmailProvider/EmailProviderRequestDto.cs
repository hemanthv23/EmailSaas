namespace EmailSaas.Application.DTOs.EmailProvider;

public class EmailProviderRequestDto
{
    public int ClientId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string? ReplyToEmail { get; set; }
    public string? SmtpHost { get; set; }
    public int? SmtpPort { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? ApiKey { get; set; }
    public bool IsDefault { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}