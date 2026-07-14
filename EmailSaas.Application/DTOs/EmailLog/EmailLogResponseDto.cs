using System;

namespace EmailSaas.Application.DTOs.EmailLog
{
    public class EmailLogResponseDto
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }
        public string ApplicationCode { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;

        public int ClientId { get; set; }
        public string ClientCode { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;

        public int TemplateId { get; set; }
        public string TemplateCode { get; set; } = string.Empty;
        public string TemplateName { get; set; } = string.Empty;

        public string ToEmail { get; set; } = string.Empty;
        public string? CcEmail { get; set; }
        public string? BccEmail { get; set; }

        // Email Content
        public string Subject { get; set; } = string.Empty;
        public string? ParameterValues { get; set; }
        public string? RenderedBody { get; set; }

        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public DateTime? SentDate { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Webhook Tracking
        public string? MessageId { get; set; }
        public string? WebhookStatus { get; set; }

        // Delivery Tracking
        public DateTime? DeliveredAt { get; set; }

        // Open Tracking
        public DateTime? OpenedAt { get; set; }
        public DateTime? LastOpenedAt { get; set; }
        public int OpenCount { get; set; }

        // Click Tracking
        public DateTime? ClickedAt { get; set; }
        public DateTime? LastClickedAt { get; set; }
        public int ClickCount { get; set; }

        // Attachment Tracking
        public bool HasAttachment { get; set; }
        public string? AttachmentNames { get; set; }
        public DateTime? AttachmentOpenedAt { get; set; }
        public int AttachmentOpenCount { get; set; }

        // Bounce Tracking
        public DateTime? BouncedAt { get; set; }
        public string? BounceReason { get; set; }
    }
}