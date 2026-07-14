using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.DTOs.Webhook
{
    public class WebhookSubscriptionResponseDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string CallbackUrl { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty; // shown only once on create — see note below
        public List<string> EventTypes { get; set; } = new();
        public byte Status { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
