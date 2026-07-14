using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Domain.Enums
{
    public enum WebhookEventType : byte
    {
        Delivered = 1,
        Opened = 2,
        Clicked = 3,
        Bounced = 4,
        Failed = 5
    }
}
