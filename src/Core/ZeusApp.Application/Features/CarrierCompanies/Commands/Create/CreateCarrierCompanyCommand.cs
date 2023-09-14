﻿using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using ZeusApp.Application.Interfaces.Repositories;
using ZeusApp.Domain.Entities.Catalog;


namespace ZeusApp.Application.Features.CarrierCompanies.Commands.Create;
public class CreateCarrierCompanyCommand : IRequest<Result<int>>
{
    public string Name { get; set; }
}
public class CreateCarrierCompanyCommandHandler : IRequestHandler<CreateCarrierCompanyCommand, Result<int>>
{
    private readonly ICarrierCompanyRepository _CarrierCompanyRepository;
    private IUnitOfWork _unitOfWork { get; set; }
    private readonly IMapper _mapper;

    public CreateCarrierCompanyCommandHandler(ICarrierCompanyRepository CarrierCompanyRepository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _CarrierCompanyRepository = CarrierCompanyRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(CreateCarrierCompanyCommand request, CancellationToken cancellationToken)
    {
        var response = _mapper.Map<CarrierCompany>(request);
        await _CarrierCompanyRepository.InsertAsync(response);
        await _unitOfWork.Commit(cancellationToken);
        return Result<int>.Success(response.Id, 200);
    }
}