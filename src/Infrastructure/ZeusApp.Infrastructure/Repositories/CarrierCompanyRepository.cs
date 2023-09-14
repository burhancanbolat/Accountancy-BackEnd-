using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreHero.Abstractions.Enums;
using Microsoft.EntityFrameworkCore;
using ZeusApp.Application.Interfaces.Repositories;
using ZeusApp.Domain.Entities.Catalog;
using ZeusApp.Infrastructure.DbContexts;

namespace ZeusApp.Infrastructure.Repositories;
public class CarrierCompanyRepository : ICarrierCompanyRepository
{

    private readonly IRepositoryAsync<CarrierCompany> _carrierCompanyRepository;
    public CarrierCompanyRepository(IRepositoryAsync<CarrierCompany> _repository)
    {
        _repository = _carrierCompanyRepository;


    }
    public IQueryable<CarrierCompany> CarrierCompanies => _carrierCompanyRepository.Entities.Where(p => p.Status != EntityStatus.Deleted).OrderBy(o => o.DisplayOrder).ThenBy(o => o.Id);

    public async Task DeleteAsync(CarrierCompany carriercompany)
    {
        carriercompany.Status = EntityStatus.Deleted;
        await _carrierCompanyRepository.UpdateAsync(carriercompany);
        //await _repository.DeleteAsync(Contact);
    }

    public async Task<CarrierCompany> GetByIdAsync(int carriercompanyId)
    {
        return await _carrierCompanyRepository.Entities.Where(p => p.Id == carriercompanyId & p.Status != EntityStatus.Deleted).FirstOrDefaultAsync();
    }

    public async Task<List<CarrierCompany>> GetListAsync()
    {
        return await _carrierCompanyRepository.Entities.Where(p => p.Status != EntityStatus.Deleted).OrderBy(o => o.DisplayOrder).ThenBy(o => o.Id).ToListAsync();
    }

    public async Task<int> InsertAsync(CarrierCompany carriercompany)
    {
        await _carrierCompanyRepository.AddAsync(carriercompany);
        return carriercompany.Id;
    }

    public async Task UpdateAsync(CarrierCompany carrierCompany)
    {
        await _carrierCompanyRepository.UpdateAsync(carrierCompany);
    }
}
