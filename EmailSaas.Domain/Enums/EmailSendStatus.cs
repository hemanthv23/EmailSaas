using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Domain.Enums
{
    public enum EmailSendStatus : byte
    {
        Pending = 0,
        Sent = 1,
        Failed = 2,
        Delivered = 3,
        Bounced = 4
    }
}
