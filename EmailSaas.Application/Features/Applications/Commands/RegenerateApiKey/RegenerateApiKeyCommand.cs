using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Applications.Commands.RegenerateApiKey
{
    public class RegenerateApiKeyCommand : IRequest<Result<ApplicationResponseDto>>
    {
        public string ApplicationCode { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
    }
}
