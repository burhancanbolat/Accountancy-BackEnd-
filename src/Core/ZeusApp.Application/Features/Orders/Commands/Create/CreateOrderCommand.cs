﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Results;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ZeusApp.Application.DTOs.Order;
using ZeusApp.Application.Features.Invoices.Commands.Create;
using ZeusApp.Application.Features.OrderCategories.Commands.Create;
using ZeusApp.Application.Interfaces.Repositories;
using ZeusApp.Domain.Entities.Catalog;
using ZeusApp.Domain.Enums;

namespace ZeusApp.Application.Features.Orders.Commands.Create;
public class CreateOrderCommand : IRequest<Result<CreateOrderCommand>>
{

    /// <summary>
    /// Sipariş Tarihi
    /// </summary>
    public DateTime OrderDate { get; set; }


    /// <summary>
    ///Sipariş Numarası
    /// </summary>
    public string? OrderNumber { get; set; }

    /// <summary>
    /// Döviz
    /// </summary>
    public CurrencyType CurrencyType { get; set; }

    /// <summary>
    /// Döviz Kuru
    /// </summary>
    public decimal ExchangeRate { get; set; }



    /// <summary>
    /// Açıklama
    /// </summary>
    public string? Description { get; set; }


    /// <summary>
    /// Sipariş Kategorisi
    /// </summary>
    public int? OrderCategoryId { get; set; }


    /// <summary>
    /// Ara Toplam
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// İndirim Tipi
    /// </summary>
    public DiscountType DiscountType { get; set; }


    /// <summary>
    /// İndirim tutarı
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Toplam İndirim
    /// </summary>
    public decimal TotalDiscount { get; set; }

    /// <summary>
    /// Genel Toplam
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Toplam KDV Tutarı
    /// </summary>
    public decimal TotalVATAmount { get; set; }


    /// <summary>
    /// Toplam :İndirimlerin düşürülmüş tutarı
    /// </summary>
    public decimal Total { get; set; }



    /// <summary>
    ///Sipariş Tipi-Toptan mı , Perakande mi?
    ///Toptansa Kvd hariç, Perakende ise kdv hariç
    ///Sadece Sipariş için gönderilecek.
    /// </summary>
    public OrderInvoiceType OrderInvoiceType { get; set; }


    /// <summary>
    ///Satış sipariş mi, alış faturası mi?
    /// </summary>
    public InvoiceType InvoiceType { get; set; }


    /// <summary>
    ///Kalan Tutar Tabloda göstereceğiz
    /// </summary>
    public decimal RemainingAmount { get; set; }


    /// <summary>
    /// Taşıyıcı Firma
    /// </summary>
    public int? CarrierCompanyId { get; set; }

    //Seri veya Lot numarası Seri veya lot numarası son kullanma tarihi
    /// <summary>
    /// Müşteri
    /// </summary>
    public int CustomerSupplierId { get; set; }

    /// <summary>
    /// Ödemeli mi?
    /// </summary>
    public bool HasPaid { get; set; }

    /// <summary>
    /// Müşteri Sipariş Numarası
    /// Sadece Satış siparişde var.
    /// </summary>
    public string? CustomerOrderNumber { get; set; }

    // <summary>
    /// Tedarikçi Sipariş Numarası
    /// Sadece Alış siparişde var.
    /// </summary>
    public string? SupplierOrderNumber { get; set; }


    /// <summary>
    /// Sipariş Durumu
    /// </summary>
    public OrderStatus OrderStatus { get; set; }


    /// <summary>
    /// Ürünler-Hizmet ve sipariş  arasında çoka çok ilişki ve tutulması gereken değerle burada tutuluyor.
    /// </summary>
    public ICollection<CreateProductOrderRequest> ProductOrders { get; set; } = new HashSet<CreateProductOrderRequest>();




}
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<CreateOrderCommand>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductServiceRepository _productServiceRepository;
    private readonly IMapper _mapper;
    private readonly ICustomerSupplierRepository _customerSupplierRepository;
    private IUnitOfWork _unitOfWork { get; set; }

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, IUnitOfWork unitOfWork, IProductServiceRepository productServiceRepository, ICustomerSupplierRepository customerSupplierRepository)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _productServiceRepository = productServiceRepository;
        _customerSupplierRepository = customerSupplierRepository;
    }

    public async Task<Result<CreateOrderCommand>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.ProductOrders.Count == 0 || request.ProductOrders.Any(x => x.ProductServiceId == 0 || x.ProductServiceId == null))
        {
            throw new Exception("Lütfen ürün seçiniz!");
        }

        if (request.CurrencyType != CurrencyType.TL && request.ExchangeRate is 0)
            throw new Exception("Lütfen döviz kuru giriniz!");


        if (request.InvoiceType == InvoiceType.selling)
        {
            //Toplam tutar hesaplama
            //Toplam indirimi tutar cinsinden al.

            //Hesaplama==Doğru
            if (request.OrderInvoiceType == OrderInvoiceType.PerakendeSiparisFaturasıKDVDahil)
            {
                decimal totalAmountDiscount = 0;
                foreach (var p in request.ProductOrders)
                {
                    p.TotalSalesAmountForProduct = p.ProductAmount * p.UnitPrice;

                    //İndirimi hesapla ve  toplama tutardan çıkar.
                    if (p.Discount != 0)
                    {
                        if (p.DiscountType == DiscountType.Amount)
                        {
                            p.TotalSalesAmountForProduct -= p.Discount;
                            totalAmountDiscount += p.Discount;
                        }
                        else
                        {
                            p.Discount = p.Discount > 100 ? 100 : p.Discount;

                            var discountAmountt = (p.TotalSalesAmountForProduct * p.Discount) / 100;
                            p.TotalSalesAmountForProduct = p.TotalSalesAmountForProduct - discountAmountt;
                            totalAmountDiscount += discountAmountt;
                        }
                    }
                    //ürüne ait vergi tutarını hesapla

                    decimal vatCalculate = (1 + (Convert.ToDecimal(p.TaxRate) / 100));

                    p.TaxAmount = p.TotalSalesAmountForProduct - (p.TotalSalesAmountForProduct / vatCalculate);
                }

                //Genel Toplam 
                request.TotalAmount = request.ProductOrders.Sum(x => x.TotalSalesAmountForProduct);

                decimal discountAmount = 0;
                if (request.DiscountAmount != 0 && request.DiscountType == DiscountType.Ratio)
                {
                    request.DiscountAmount = request.DiscountAmount > 100 ? 100 : request.DiscountAmount;

                    discountAmount = request.TotalAmount * (request.DiscountAmount / 100);
                }
                else if (request.DiscountAmount != 0 && request.DiscountType == DiscountType.Amount)
                {
                    discountAmount = request.DiscountAmount;
                }

                //Ara Toplam= Dikkat!! Faturadaki genel toplam indirim ara toplamdan düşürülmez.
                request.Subtotal = request.ProductOrders.Sum(x => x.TotalSalesAmountForProduct);

                if (request.DiscountAmount != 0)
                {
                    foreach (var x in request.ProductOrders)
                    {
                        // Her bir ürüne düşecek indirim miktarı:
                        //Her ürüne ait toplam  fiyat/ Ara toplam * indirim fiyatı

                        decimal productDiscount = (x.TotalSalesAmountForProduct / request.Subtotal) * discountAmount;
                        x.TotalSalesAmountForProduct -= productDiscount;

                        //ürüne ait vergi tutarını tekrardan hesapla
                        decimal vatCalculate = (1 + (Convert.ToDecimal(x.TaxRate) / 100));

                        x.TaxAmount = x.TotalSalesAmountForProduct - (x.TotalSalesAmountForProduct / vatCalculate);

                        //Beklenen miktar en başta ProductAmount'ın eşittir.
                        x.PendingQuantity = x.ProductAmount;
                    }
                }

                //Güncel Genel toplamı hesapla 
                request.TotalAmount = request.ProductOrders.Sum(x => x.TotalSalesAmountForProduct);

                //indirimi toplam tutardan çıkar
                //request.TotalAmount -= discountAmount;

                //Toplam indirimi hesapla
                request.TotalDiscount = totalAmountDiscount + discountAmount;

                //Toplam Kdv Tutarı
                request.TotalVATAmount = request.ProductOrders.Sum(x => x.TaxAmount);

                //Toplam= (Genel Toplam - Toplam Kdv Tutarı)
                request.Total = request.TotalAmount - request.TotalVATAmount;

                //ilgili ürünlerin stoğunu güncelle.= true;

                //foreach (var x in request.ProductOrders)
                //{
                //    var product = _productServiceRepository.ProductServices.SingleOrDefault(p => p.Id == x.ProductServiceId);
                //    if (product!.ProductServiceType == ProductServiceType.product)
                //    {
                //        product!.TotalStockAmount -= x.ProductAmount;
                //        await _productServiceRepository.UpdateAsync(product);
                //    }
                //}
                request.RemainingAmount = request.TotalAmount;
            }


            //Eğerki gelen değer satış ve toptan satışssa
            //Hesaplama==Doğru
            if (request.OrderInvoiceType == OrderInvoiceType.ToptanSiparisFaturasiKDVHaric)
            {
                request = Calculate(request);

                //ilgili ürünlerin stoğunu güncelle.= true;

                //foreach (var x in request.ProductOrders)
                //{
                //    var product = _productServiceRepository.ProductServices.SingleOrDefault(p => p.Id == x.ProductServiceId);
                //    if (product!.ProductServiceType == ProductServiceType.product)
                //    {
                //        product!.TotalStockAmount -= x.ProductAmount;
                //        await _productServiceRepository.UpdateAsync(product);
                //    }
                //}
                request.RemainingAmount = request.TotalAmount;
            }

            decimal exchangeTotal = request.TotalAmount;

            if (request.CurrencyType != CurrencyType.TL)
            {
                exchangeTotal = request.ExchangeRate * request.TotalAmount;
            }

            //Eğerki ödemeli true ise müşteri bakiyesi artar.

            if (request.HasPaid)
            {
                //Müşteriye borç eklenecek.=true;
                var customerOrSupplier = await _customerSupplierRepository.CustomerSuppliers.SingleOrDefaultAsync(x => x.Id == request.CustomerSupplierId);
                customerOrSupplier!.TotalBalance -= exchangeTotal;
                await _customerSupplierRepository.UpdateAsync(customerOrSupplier);
            }
        }

        //Eğerki alış faturası seçildiyse
        //Hesaplama==Doğru
        if (request.InvoiceType == InvoiceType.buying)
        {
            request = Calculate(request);

            //ilgili ürünlerin stoğunu güncelle.= true;

            //foreach (var x in request.ProductOrders)
            //{
            //    var product = _productServiceRepository.ProductServices.SingleOrDefault(p => p.Id == x.ProductServiceId);
            //    if (product!.ProductServiceType == ProductServiceType.product)
            //    {
            //        product!.TotalStockAmount += x.ProductAmount;
            //        await _productServiceRepository.UpdateAsync(product);
            //    }
            //}


            //Döviz eğer tl değilse tl'ye çevir.
            decimal exchangeTotal = request.TotalAmount;

            if (request.CurrencyType != CurrencyType.TL)
            {
                exchangeTotal = request.ExchangeRate * request.TotalAmount;
            }

            //Eğerki ödemeli true ise müşteri bakiyesi artar.

            if (request.HasPaid)
            {
                //Müşteriye borç eklenecek.=true;
                var customerOrSupplier = await _customerSupplierRepository.CustomerSuppliers.SingleOrDefaultAsync(x => x.Id == request.CustomerSupplierId);
                customerOrSupplier!.TotalBalance -= exchangeTotal;
                await _customerSupplierRepository.UpdateAsync(customerOrSupplier);
            }
            request.RemainingAmount = request.TotalAmount;
        }

          
        var order = _mapper.Map<Order>(request);

        await _orderRepository.InsertAsync(order);

        await _unitOfWork.Commit(cancellationToken);
        //   return Result<>.Success(invoice, 200);


        return await Result<CreateOrderCommand>.SuccessAsync(request, 200);

        //Eğer hasPaid true ise müşterinin bakiyesini arttır.

        //var orderCategory = _mapper.Map<OrderCategory>(request); 
        //await _unitOfWork.Commit(cancellationToken);
    }


    //Eğerki gelen değer satış ve toptan satışssa ilgili hesaplamaları yapar.
    //Eğerki alış faturası seçildiyse ilgili hesaplamaları yapar.
    public CreateOrderCommand Calculate(CreateOrderCommand request)
    {
        decimal totalDiscount = 0;
        //Toplam tutar hesaplama- //Vergiler hariç
        request.ProductOrders.ToList().ForEach(p =>
        {
            p.TotalSalesAmountForProduct = p.ProductAmount * p.UnitPrice;

            //İndirimi hesapla ve  toplama tutardan çıkar.

            if (p.Discount != 0)
            {
                if (p.DiscountType == DiscountType.Amount)
                {
                    p.TotalSalesAmountForProduct -= p.Discount;
                    totalDiscount += p.Discount;
                }
                else
                {
                    p.Discount = p.Discount > 100 ? 100 : p.Discount;

                    var discountAmount = (p.TotalSalesAmountForProduct * p.Discount) / 100;
                    p.TotalSalesAmountForProduct = p.TotalSalesAmountForProduct - discountAmount;
                    totalDiscount += discountAmount;
                }
            }

            //ürüne ait vergi tutarını hesapla                   
            p.TaxAmount = p.TotalSalesAmountForProduct * (Convert.ToDecimal(p.TaxRate) / 100);
        });


        //Genel Toplam =Tüm productların (TotalSalesAmountForProduct + vetgi tutarı)
        //var productAllAmount = request.ProductInvoices.Sum(x => x.TotalSalesAmountForProduct);
        var taxAmount = request.ProductOrders.Sum(x => x.TaxAmount);

        //request.TotalAmount = productAllAmount + taxAmount;

        //Toplam= (Genel Toplam - Toplam Kdv Tutarı)
        request.Total = request.ProductOrders.Sum(x => x.TotalSalesAmountForProduct);
        //x
        decimal discountAmount = 0;
        if (request.DiscountAmount != 0 && request.DiscountType == DiscountType.Ratio)
        {
            request.DiscountAmount = request.DiscountAmount > 100 ? 100 : request.DiscountAmount;

            discountAmount = request.Total * (request.DiscountAmount / 100);
        }
        else if (request.DiscountAmount != 0 && request.DiscountType == DiscountType.Amount)
        {
            discountAmount = request.DiscountAmount;
        }

        //Ara Toplam= Dikkat!! Faturadaki genel toplam indirim ara toplamdan düşürülmez.
        request.Subtotal = request.ProductOrders.Sum(x => x.TotalSalesAmountForProduct) + totalDiscount;


        //Dikkat hatalı.
        if (request.DiscountAmount != 0)
        {
            foreach (var x in request.ProductOrders)
            {
                // Her bir ürüne düşecek indirim miktarı:
                //Her ürüne ait toplam  fiyat + ürüne özel indirim fiyatı / Ara toplam * indirim fiyatı

                decimal notDiscountProductTotalPrice = 0;
                if (x.Discount != 0)
                {
                    if (x.DiscountType == DiscountType.Amount)
                    {
                        // İndirimsiz fiyat hesapla
                        notDiscountProductTotalPrice = x.TotalSalesAmountForProduct + x.Discount;
                    }
                    else
                    {
                        x.Discount = x.Discount > 100 ? 100 : x.Discount;

                        //İndirimsiz fiyat hesapla
                        notDiscountProductTotalPrice = x.TotalSalesAmountForProduct / (1 - (x.Discount / 100));

                    }
                }

                //  Her ürüne ait toplam  fiyat + ürüne özel indirim fiyatı / Ara toplam* indirim fiyatı
                decimal productDiscount = (notDiscountProductTotalPrice / request.Subtotal) * discountAmount;

                x.TotalSalesAmountForProduct -= productDiscount;

                //ürüne ait vergi tutarını tekrardan hesapla
                x.TaxAmount = x.TotalSalesAmountForProduct * (Convert.ToDecimal(x.TaxRate) / 100);

                //Beklenen miktar en başta ProductAmount'ın eşittir.
                x.PendingQuantity = x.ProductAmount;
            }
        }

        //Toplam Kdv Tutarı
        request.TotalVATAmount = request.ProductOrders.Sum(x => x.TaxAmount);

        //Güncel Genel toplamı hesapla 
        request.TotalAmount = request.ProductOrders.Sum(x => x.TotalSalesAmountForProduct) + request.TotalVATAmount;

        //indirimi toplam tutardan çıkar
        //request.TotalAmount -= discountAmount;

        //Toplam indirimi hesapla
        request.TotalDiscount = totalDiscount + discountAmount;

        //Toplam= (Genel Toplam - Toplam Kdv Tutarı)
        request.Total = request.TotalAmount - request.TotalVATAmount;
        //    var invoice = _mapper.Map<Invoice>(request);
        request.TotalAmount = request.Total + request.TotalVATAmount;

        return request;
    }
}



//   if (request.ProductInvoices.Count == 0 || request.ProductInvoices.Any(x => x.ProductServiceId == 0 || x.ProductServiceId == null))
//        {
//            throw new Exception("Lütfen ürün seçiniz!");
//        }

//        if (request.CurrencyType != CurrencyType.TL && request.ExchangeRate is 0)
//{
//    throw new Exception("Lütfen döviz kuru giriniz!");
//}

////Eğerki isOtherAdress true ise 

////Eğerki gelen değer satış ve perakende satışssa = Bitti
//if (request.InvoiceType == InvoiceType.selling)
//{
//    if (request.SalesInvoiceType == SalesInvoiceType.PerakendeSatisFaturasıKDVDahil)
//    {
//        //Toplam tutar hesaplama
//        //Toplam indirimi tutar cinsinden al.

//        decimal totalAmountDiscount = 0;
//        foreach (var p in request.ProductInvoices)
//        {
//            p.TotalSalesAmountForProduct = p.ProductAmount * p.UnitPrice;

//            //İndirimi hesapla ve  toplama tutardan çıkar.
//            if (p.Discount != 0)
//            {
//                if (p.DiscountType == DiscountType.Amount)
//                {
//                    p.TotalSalesAmountForProduct -= p.Discount;
//                    totalAmountDiscount += p.Discount;
//                }
//                else
//                {
//                    p.Discount = p.Discount > 100 ? 100 : p.Discount;

//                    var discountAmountt = (p.TotalSalesAmountForProduct * p.Discount) / 100;
//                    p.TotalSalesAmountForProduct = p.TotalSalesAmountForProduct - discountAmountt;
//                    totalAmountDiscount += discountAmountt;
//                }
//            }
//            //ürüne ait vergi tutarını hesapla

//            decimal vatCalculate = (1 + (Convert.ToDecimal(p.TaxRate) / 100));

//            p.TaxAmount = p.TotalSalesAmountForProduct - (p.TotalSalesAmountForProduct / vatCalculate);
//        }


//        //request.TotalDiscount=request.ProductInvoices.Sum(x=>x.)


//        //Genel Toplam 
//        request.TotalAmount = request.ProductInvoices.Sum(x => x.TotalSalesAmountForProduct);

//        decimal discountAmount = 0;
//        if (request.DiscountAmount != 0 && request.DiscountType == DiscountType.Ratio)
//        {
//            request.DiscountAmount = request.DiscountAmount > 100 ? 100 : request.DiscountAmount;

//            discountAmount = request.TotalAmount * (request.DiscountAmount / 100);
//        }
//        else if (request.DiscountAmount != 0 && request.DiscountType == DiscountType.Amount)
//        {
//            discountAmount = request.DiscountAmount;
//        }


//        //Ara Toplam= Dikkat!! Faturadaki genel toplam indirim ara toplamdan düşürülmez.
//        request.Subtotal = request.ProductInvoices.Sum(x => x.TotalSalesAmountForProduct);

//        if (request.DiscountAmount != 0)
//        {
//            foreach (var x in request.ProductInvoices)
//            {
//                // Her bir ürüne düşecek indirim miktarı:
//                //Her ürüne ait toplam  fiyat/ Ara toplam * indirim fiyatı

//                decimal productDiscount = (x.TotalSalesAmountForProduct / request.Subtotal) * discountAmount;
//                x.TotalSalesAmountForProduct -= productDiscount;

//                //ürüne ait vergi tutarını tekrardan hesapla
//                decimal vatCalculate = (1 + (Convert.ToDecimal(x.TaxRate) / 100));

//                x.TaxAmount = x.TotalSalesAmountForProduct - (x.TotalSalesAmountForProduct / vatCalculate);
//            }
//        }
//        //Güncel Genel toplamı hesapla 
//        request.TotalAmount = request.ProductInvoices.Sum(x => x.TotalSalesAmountForProduct);

//        //indirimi toplam tutardan çıkar
//        //request.TotalAmount -= discountAmount;

//        //Toplam indirimi hesapla
//        request.TotalDiscount = totalAmountDiscount + discountAmount;

//        //Toplam Kdv Tutarı
//        request.TotalVATAmount = request.ProductInvoices.Sum(x => x.TaxAmount);

//        //Toplam= (Genel Toplam - Toplam Kdv Tutarı)
//        request.Total = request.TotalAmount - request.TotalVATAmount;

//        //ilgili ürünlerin stoğunu güncelle.= true;

//        foreach (var x in request.ProductInvoices)
//        {
//            var product = _productServiceRepository.ProductServices.SingleOrDefault(p => p.Id == x.ProductServiceId);
//            if (product!.ProductServiceType == ProductServiceType.product)
//            {
//                product!.TotalStockAmount -= x.ProductAmount;
//                await _productServiceRepository.UpdateAsync(product);
//            }
//        }
//        request.RemainingAmount = request.TotalAmount;
//    }

//--------------------------------------------------------------------------------------------------
//    //Eğerki gelen değer satış ve toptan satışssa
//    if (request.SalesInvoiceType == SalesInvoiceType.ToptanSatisFaturasiKDVHaric)
//    {
//        request = Calculate(request);

//        //ilgili ürünlerin stoğunu güncelle.= true;

//        foreach (var x in request.ProductInvoices)
//        {
//            var product = _productServiceRepository.ProductServices.SingleOrDefault(p => p.Id == x.ProductServiceId);
//            if (product!.ProductServiceType == ProductServiceType.product)
//            {
//                product!.TotalStockAmount -= x.ProductAmount;
//                await _productServiceRepository.UpdateAsync(product);
//            }
//        }
//        request.RemainingAmount = request.TotalAmount;
//    }

//}

////Eğerki alış faturası seçildiyse
//if (request.InvoiceType == InvoiceType.buying)
//{
//    request = Calculate(request);

//    //ilgili ürünlerin stoğunu güncelle.= true;

//    foreach (var x in request.ProductInvoices)
//    {
//        var product = _productServiceRepository.ProductServices.SingleOrDefault(p => p.Id == x.ProductServiceId);
//        if (product!.ProductServiceType == ProductServiceType.product)
//        {
//            product!.TotalStockAmount += x.ProductAmount;
//            await _productServiceRepository.UpdateAsync(product);
//        }
//    }

//    request.RemainingAmount = request.TotalAmount;
//}


////Müşteriye borç eklenecek.=true;
//var customerOrSupplier = await _customerSupplierRepository.CustomerSuppliers.SingleOrDefaultAsync(x => x.Id == request.CustomerSupplierId);

////Döviz eğer tl değilse tl'ye çevir.
//decimal exchangeTotal = request.TotalAmount;

//if (request.CurrencyType != CurrencyType.TL)
//{
//    exchangeTotal = request.ExchangeRate * request.TotalAmount;
//}

//customerOrSupplier!.TotalBalance += exchangeTotal;

//await _customerSupplierRepository.UpdateAsync(customerOrSupplier);

//var invoice = _mapper.Map<Invoice>(request);

//await _invoiceRepository.InsertAsync(invoice);

//await _unitOfWork.Commit(cancellationToken);
//return Result<Invoice>.Success(invoice, 200);