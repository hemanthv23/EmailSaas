
using EmailSaas.Application.Common.Models;
using EmailSaas.Application.DTOs.Application;
using MediatR;

namespace EmailSaas.Application.Features.Applications.Commands.CreateApplication;

public class CreateApplicationCommand
    : IRequest<Result<ApplicationResponseDto>>
{
    public string ApplicationCode { get; set; } = string.Empty;

    public string ApplicationName { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;
}