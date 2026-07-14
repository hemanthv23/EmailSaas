using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.EmailProvider;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.EmailProviders.Queries.GetAllEmailProviders
{
    public class GetAllEmailProvidersQuery : IRequest<Result<List<EmailProviderResponseDto>>>
    {
        public int ApplicationId { get; set; } // set by controller from ApiKey
    }
}
