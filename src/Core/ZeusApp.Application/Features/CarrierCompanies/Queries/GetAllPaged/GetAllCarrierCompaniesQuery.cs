using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using ZeusApp.Application.Interfaces.Repositories;

namespace ZeusApp.Application.Features.CarrierCompanies.Queries.GetAllPaged;

public class GetAllCarrierCompaniesQuery : IRequest<Result<List<GetAllCarrierCompaniesResponse>>>
{

}
public class GetAllCarrierCompaniesQueryHandler : IRequestHandler<GetAllCarrierCompaniesQuery, Result<List<GetAllCarrierCompaniesResponse>>>
{
    private readonly ICarrierCompanyRepository _carrierCompanyRepository;
    private readonly IMapper _mapper;

    public GetAllCarrierCompaniesQueryHandler(ICarrierCompanyRepository CarrierCompanyRepository, IMapper mapper)
    {
        _carrierCompanyRepository = CarrierCompanyRepository;
        _mapper = mapper;
    }
    public async Task<Result<List<GetAllCarrierCompaniesResponse>>> Handle(GetAllCarrierCompaniesQuery request, CancellationToken cancellationToken)
    {
        var CarrierCompanies = await _carrierCompanyRepository.GetListAsync();
        var CarrierCompaniesResponse = _mapper.Map<List<GetAllCarrierCompaniesResponse>>(CarrierCompanies);
        return Result<List<GetAllCarrierCompaniesResponse>>.Success(CarrierCompaniesResponse, 200);
    }
}