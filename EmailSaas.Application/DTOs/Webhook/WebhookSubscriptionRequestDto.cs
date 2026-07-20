using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.DTOs.Webhook
{
    public class WebhookSubscriptionRequestDto
    {
        public int ClientID { get; set; }
        public string CallbackUrl { get; set; } = string.Empty;
        public List<string> EventTypes { get; set; } = new(); // e.g. ["Delivered","Opened","Clicked","Bounced"]
        public string CreatedBy { get; set; } = string.Empty;
    }
}
