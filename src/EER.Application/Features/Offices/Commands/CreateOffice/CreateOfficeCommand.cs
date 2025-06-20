﻿using MediatR;

namespace EER.Application.Features.Offices.Commands.CreateOffice;

public record CreateOfficeCommand(CreateOfficeDto CreateOfficeDto, Guid Manipulator) : IRequest<OfficeCreatedDto>;
