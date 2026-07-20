using EmailSaas.Domain.Common;

namespace EmailSaas.Domain.Entities;

public class EmailEventLog : AuditableEntity
{
    public int LogID { get; set; }
    public string MessageID { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string? LogData { get; set; }
    public DateTime EventLogDate { get; set; }
    public byte Status { get; set; }

    public EmailLog EmailLog { get; set; } = null!;
}