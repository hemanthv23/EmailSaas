using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.DTOs.Webhook
{
    public class WebhookSubscriptionUpdateDto
    {
        public string CallbackUrl { get; set; } = string.Empty;
        public List<string> EventTypes { get; set; } = new();
        public byte Status { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
