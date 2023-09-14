using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using ZeusApp.Application.DTOs.Expense;
using ZeusApp.Application.DTOs.Stock;
using ZeusApp.Application.Interfaces.Repositories;
using ZeusApp.Domain.Entities.Catalog;
using ZeusApp.Domain.Enums;

namespace ZeusApp.Application.Features.Expenses.Commands.Create;
public class CreateExpenseCommand : IRequest<Result<int>>
{
//    public int? CustomerSupplierId { get; set; }
//   // public CustomerSupplier CustomerSupplier { get; set; }

//    public DateTime InvoiceDate { get; set; } // Fiş / Fatura Tarihi

//    public string InvoiceNo { get; set; } //  Fiş / Fatura No

//    public CurrencyType? Currency { get; set; } //Döviz
//    public bool PaymentStatus { get; set; }// Ödeme Durumu
  
//    public string Description { get; set; }// Açıklama
//    public int? ExpenseCategoryId { get; set; }//Gider Kategori
//    public decimal GrandTotal { get; set; }// Genel Toplam Tl veya Herhangi Döviz
//    public decimal DiscountTotal { get; set; }//  Toplam indirimi
//    public DiscountType? DiscountType { get; set; }// İndirim Türü
//    public List<ExpenseServiceCreateRequest> ExpenseServices { get; set; } = new List<ExpenseServiceCreateRequest>();
}
//public class CreateExpenseCommandHandler : IRequestHandler<CreateExpenseCommand, Result<int>>
//{
//    private readonly IExpenseRepository _expenseRepository;
//    private readonly IProductServiceRepository _serviceRepository;
//    private readonly IMapper _mapper;
//    private IUnitOfWork _unitOfWork { get; set; }

//    public CreateExpenseCommandHandler(IExpenseRepository expenseRepository, IProductServiceRepository serviceRepository, IMapper mapper, IUnitOfWork unitOfWork)
//    {
//        _expenseRepository = expenseRepository;
//        _serviceRepository = serviceRepository;
//        _mapper = mapper;
//        _unitOfWork = unitOfWork;
//    }

//    public Task<Result<int>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
//    {
//        if (request.ExpenseServices.Count == 0)
//        {
//            throw new Exception("En az bir ürün girilmelidir.");
//        }

//        throw new NotImplementedException();
//    }
//}
