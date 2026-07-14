using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.DTOs.Application
{
    public class ApplicationRequestDto
    {
        public string ApplicationCode { get; set; } = string.Empty;
        public string ApplicationName { get; set; } = string.Empty;
    }
}
