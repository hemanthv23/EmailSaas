using MediatR;
using Microsoft.EntityFrameworkCore;
using EmailSaas.Application.Common.Interfaces;
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.Application;
using EmailSaas.Domain.Entities;
using EmailSaas.Domain.Enums;

namespace EmailSaas.Application.Features.Applications.Commands.CreateApplication;

public class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand, Result<ApplicationResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public CreateApplicationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ApplicationResponseDto>> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.MasterApplications
            .AnyAsync(x => x.ApplicationCode == request.ApplicationCode, cancellationToken);

        if (exists)
            return Result<ApplicationResponseDto>.Failure($"ApplicationCode '{request.ApplicationCode}' already exists.");

        var entity = new MasterApplication
        {
            ApplicationCode = request.ApplicationCode,
            ApplicationName = request.ApplicationName,
            ApiKey = GenerateApiKey(),
            Status = (byte)CommonStatus.Active,
            CreatedBy = string.IsNullOrWhiteSpace(request.CreatedBy)
        ? "admin"
        : request.CreatedBy,
            CreatedDate = DateTime.UtcNow
        };

        _context.MasterApplications.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new ApplicationResponseDto
        {
            Id = entity.Id,
            ApplicationCode = entity.ApplicationCode,
            ApplicationName = entity.ApplicationName,
            ApiKey = entity.ApiKey,
            Status = entity.Status,
            CreatedBy = entity.CreatedBy,
            CreatedDate = entity.CreatedDate,
            UpdatedBy = entity.UpdatedBy,
            UpdatedDate = entity.UpdatedDate,
            DeletedBy = entity.DeletedBy,
            DeletedDate = entity.DeletedDate
        };

        return Result<ApplicationResponseDto>.Success(response);
    }


    //GENERATE API KEY
    private static string GenerateApiKey()
    {
        return $"ESAAS-{Guid.NewGuid():N}{Guid.NewGuid():N}".ToUpper()[..64];
    }
}