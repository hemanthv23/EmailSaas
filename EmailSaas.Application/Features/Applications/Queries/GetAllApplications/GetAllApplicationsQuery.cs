using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Applications.Queries.GetAllApplications
{
    public class GetAllApplicationsQuery : IRequest<Result<List<ApplicationResponseDto>>>
    {
        public int ApplicationId { get; set; } // set by controller from ApiKey
    }
}
