using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.EmailProvider;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.EmailProviders.Queries.GetEmailProviderById
{
    public class GetEmailProviderByIdQuery : IRequest<Result<EmailProviderResponseDto>>
    {
        public int Id { get; set; }
    }
}
