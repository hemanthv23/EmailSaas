using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Infrastructure.Services
{
    public class WebhookSettings
    {
        public int MaxAttempts { get; set; } = 5;
        public int PollingIntervalSeconds { get; set; } = 30;
        public int TimeoutSeconds { get; set; } = 15;
        public int BaseBackoffSeconds { get; set; } = 60;
    }
}
