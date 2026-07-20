using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.EmailLog;
using EmailSaas.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSaas.Application.Features.EmailLogs.Queries.GetAllEmailLogs
{
    public class GetAllEmailLogsQueryHandler : IRequestHandler<GetAllEmailLogsQuery, Result<List<EmailLogResponseDto>>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllEmailLogsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<EmailLogResponseDto>>> Handle(GetAllEmailLogsQuery request, CancellationToken cancellationToken)
        {
            var logs = await _context.EmailLogs
                .AsNoTracking()
                .Include(x => x.Application)
                .Include(x => x.Client)
                .Include(x => x.Template)
                .Where(x => x.ApplicationId == request.ApplicationId)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new EmailLogResponseDto
                {
                    Id = x.Id,
                    ApplicationId = x.ApplicationId,
                    ApplicationCode = x.Application.ApplicationCode,
                    ApplicationName = x.Application.ApplicationName,

                    ClientID = x.ClientID,
                    ClientCode = x.Client.ClientCode,
                    ClientName = x.Client.ClientName,
                    
                    TemplateID = x.TemplateID,
                    TemplateCode = x.Template.TemplateCode,
                    TemplateName = x.Template.TemplateName,

                    ToEmail = x.ToEmail,
                    CcEmail = x.CcEmail,
                    BccEmail = x.BccEmail,

                    Subject = x.Subject,
                    ParameterValues = x.ParameterValues,
                    RenderedBody = x.RenderedBody,

                    Status = x.Status == (byte)EmailSendStatus.Delivered ? "Delivered"
           : x.Status == (byte)EmailSendStatus.Sent ? "Sent"
           : x.Status == (byte)EmailSendStatus.Failed ? "Failed"
           : "Pending",

                    ErrorMessage = x.ErrorMessage,
                    SendDate = x.SendDate,

                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate,

                    MessageID = x.MessageID,

                    HasAttachment = x.HasAttachment,
                    AttachmentName = x.AttachmentName
                })
                .ToListAsync(cancellationToken);

            return Result<List<EmailLogResponseDto>>.Success(logs);
        }
    }
}
