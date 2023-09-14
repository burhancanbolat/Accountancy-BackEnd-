using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using MediatR;
using ZeusApp.Application.Interfaces.Repositories;
using ZeusApp.Domain.Enums;
using ZeusApp.Domain.Entities.Catalog;

namespace ZeusApp.Application.Features.CarrierCompanies.Commands.Update;

public class UpdateCarrierCompanyCommand : IRequest<Result<int>>
{
    public int Id { get; set; }
    public string Name { get; set; }
}
public class UpdateCarrierCompanyCommandHandler : IRequestHandler<UpdateCarrierCompanyCommand, Result<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICarrierCompanyRepository _CarrierCompanyRepository;

    public UpdateCarrierCompanyCommandHandler(IUnitOfWork unitOfWork, ICarrierCompanyRepository CarrierCompanyRepository)
    {
        _unitOfWork = unitOfWork;
        _CarrierCompanyRepository = CarrierCompanyRepository;
    }

    public async Task<Result<int>> Handle(UpdateCarrierCompanyCommand request, CancellationToken cancellationToken)
    {
        var CarrierCompany = await _CarrierCompanyRepository.GetByIdAsync(request.Id);
        if (CarrierCompany == null)
        {
            throw new KeyNotFoundException();
        }
        CarrierCompany.Name = request.Name;
        await _CarrierCompanyRepository.UpdateAsync(CarrierCompany);
        await _unitOfWork.Commit(cancellationToken);
        return await Result<int>.SuccessAsync(CarrierCompany.Id, 200);   //emin değilim...
    }
}