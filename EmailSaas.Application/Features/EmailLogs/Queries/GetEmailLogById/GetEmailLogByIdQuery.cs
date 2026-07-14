using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.EmailLog;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.EmailLogs.Queries.GetEmailLogById
{
    public class GetEmailLogByIdQuery : IRequest<Result<EmailLogResponseDto>>
    {
        public int Id { get; set; }
    }
}
