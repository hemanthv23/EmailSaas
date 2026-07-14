using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.Applications.Queries.GetAllApplicationsById
{
    public class GetApplicationByIdQuery : IRequest<Result<ApplicationResponseDto>>
    {
        public int Id { get; set; }
    }
}
