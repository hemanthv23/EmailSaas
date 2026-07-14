using EmailSaas.Domain.Common;

namespace EmailSaas.Domain.Entities;

public class EmailLog : AuditableEntity
{
    // ─── Existing fields ──────────────────────────────────────
    public int ApplicationId { get; set; }
    public int ClientId { get; set; }
    public int TemplateId { get; set; }
    public string ToEmail { get; set; } = string.Empty;
    public string? CcEmail { get; set; }
    public string? BccEmail { get; set; }
    public byte Status { get; set; }

    public string Subject { get; set; } = string.Empty;
    public string? ParameterValues { get; set; }
    public string? RenderedBody { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? SentDate { get; set; }

    // ─── Webhook tracking ─────────────────────────────────────
    public string? MessageId { get; set; }
    public string? WebhookStatus { get; set; }

    // ─── Delivery tracking ────────────────────────────────────
    public DateTime? DeliveredAt { get; set; }

    // ─── Open tracking ────────────────────────────────────────
    public DateTime? OpenedAt { get; set; }
    public DateTime? LastOpenedAt { get; set; }
    public int OpenCount { get; set; } = 0;

    // ─── Click tracking ───────────────────────────────────────
    public DateTime? ClickedAt { get; set; }
    public DateTime? LastClickedAt { get; set; }
    public int ClickCount { get; set; } = 0;

    // ─── Attachment tracking ──────────────────────────────────
    public bool HasAttachment { get; set; } = false;
    public string? AttachmentNames { get; set; }
    public DateTime? AttachmentOpenedAt { get; set; }
    public int AttachmentOpenCount { get; set; } = 0;

    // ─── Bounce tracking ──────────────────────────────────────
    public DateTime? BouncedAt { get; set; }
    public string? BounceReason { get; set; }

    // ─── Navigation properties ────────────────────────────────
    public ApplicationMaster Application { get; set; } = null!;
    public ClientMaster Client { get; set; } = null!;
    public EmailTemplateMaster Template { get; set; } = null!;

    // ─── Tracking navigation properties ───────────────────────
    public ICollection<EmailEvent> Events { get; set; } = new List<EmailEvent>();
    public ICollection<EmailLinkClick> LinkClicks { get; set; } = new List<EmailLinkClick>();
    public ICollection<WebhookDeliveryLog> WebhookDeliveryLogs { get; set; } = new List<WebhookDeliveryLog>();
}