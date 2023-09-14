using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using ZeusApp.Application.Interfaces.Repositories;

namespace ZeusApp.Application.Features.CarrierCompanies.Queries.GetById;
public class GetCarrierCompanyByIdQuery : IRequest<Result<GetCarrierCompanyByIdResponse>>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public class GetCarrierCompanyByIdQueryHandler : IRequestHandler<GetCarrierCompanyByIdQuery, Result<GetCarrierCompanyByIdResponse>>
    {
        private readonly ICarrierCompanyRepository _CarrierCompanyRepository;
        private readonly IMapper _mapper;

        public GetCarrierCompanyByIdQueryHandler(ICarrierCompanyRepository CarrierCompanyRepository, IMapper mapper)
        {
            _CarrierCompanyRepository = CarrierCompanyRepository;
            _mapper = mapper;
        }

        public async Task<Result<GetCarrierCompanyByIdResponse>> Handle(GetCarrierCompanyByIdQuery request, CancellationToken cancellationToken)
        {
            var CarrierCompany = await _CarrierCompanyRepository.GetByIdAsync(request.Id);
            var getCarrierCompanyByIdResponse = _mapper.Map<GetCarrierCompanyByIdResponse>(CarrierCompany);
            return await Result<GetCarrierCompanyByIdResponse>.SuccessAsync(getCarrierCompanyByIdResponse, 200);
        }
    }
}