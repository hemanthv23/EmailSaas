using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Domain.Enums
{
    public enum EmailEventType : byte
    {
        Sent = 1,
        Delivered = 2,
        Opened = 3,
        Clicked = 4,
        Bounced = 5,
        Failed = 6
    }
}
