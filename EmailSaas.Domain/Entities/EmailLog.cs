using EmailSaas.Domain.Common;

namespace EmailSaas.Domain.Entities;

public class EmailLog : AuditableEntity
{
    public int ApplicationId { get; set; }
    public int ClientID { get; set; }
    public int TemplateID { get; set; }
    public int ProviderID { get; set; }
    public string ToEmail { get; set; } = string.Empty;
    public string? CcEmail { get; set; }
    public string? BccEmail { get; set; }
    public byte Status { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? ParameterValues { get; set; }
    public string? RenderedBody { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? SendDate { get; set; }
    public string? MessageID { get; set; }
    public bool HasAttachment { get; set; }
    public string? AttachmentName { get; set; }

    public MasterApplication Application { get; set; } = null!;
    public MasterClient Client { get; set; } = null!;
    public MasterEmailTemplate Template { get; set; } = null!;
    public MasterEmailProvider Provider { get; set; } = null!;

    public ICollection<EmailEventLog> Events { get; set; } = new List<EmailEventLog>();
}