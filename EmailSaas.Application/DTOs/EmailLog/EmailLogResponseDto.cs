using System;

namespace EmailSaas.Application.DTOs.EmailLog
{
    public class EmailLogResponseDto
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }
        public string ApplicationCode { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;

        public int ClientID { get; set; }
        public string ClientCode { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;

        public int TemplateID { get; set; }
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
        public DateTime? SendDate { get; set; }

        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public string? MessageID { get; set; }

        public bool HasAttachment { get; set; }
        public string? AttachmentName { get; set; }
    }
}