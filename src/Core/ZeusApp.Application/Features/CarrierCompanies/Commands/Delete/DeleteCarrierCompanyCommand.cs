using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using ZeusApp.Application.Interfaces.Repositories;

namespace ZeusApp.Application.Features.CarrierCompanies.Commands.Delete;
public class DeleteCarrierCompanyCommand : IRequest<Result<int>>
{
    public int Id { get; set; }

}
public class DeleteCarrierCompanyCommandHandler : IRequestHandler<DeleteCarrierCompanyCommand, Result<int>>
{
    private readonly ICarrierCompanyRepository _CarrierCompanyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCarrierCompanyCommandHandler(ICarrierCompanyRepository CarrierCompanyRepository,
        IUnitOfWork unitOfWork, IMapper mapper)
    {
        _CarrierCompanyRepository = CarrierCompanyRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<int>> Handle(DeleteCarrierCompanyCommand request, CancellationToken cancellationToken)
    {
        var CarrierCompany = await _CarrierCompanyRepository.GetByIdAsync(request.Id);
        await _CarrierCompanyRepository.DeleteAsync(CarrierCompany);
        await _unitOfWork.Commit(cancellationToken);
        return await Result<int>.SuccessAsync(CarrierCompany.Id, 200);
    }
}