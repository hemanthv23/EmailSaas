namespace EmailSaas.Application.DTOs.EmailProvider;

public class EmailProviderResponseDto
{
    public int Id { get; set; }
    public int ClientID { get; set; }
    public string ClientCode { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string? ReplyToEmail { get; set; }
    public string? SMTPHost { get; set; }
    public int? SMTPPort { get; set; }
    public string? UserName { get; set; }
    public bool IsDefault { get; set; }
    public byte Status { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}