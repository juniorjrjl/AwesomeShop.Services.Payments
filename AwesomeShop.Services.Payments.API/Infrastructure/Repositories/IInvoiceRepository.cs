using AwesomeShop.Services.Payments.API.Core.Entities;

namespace AwesomeShop.Services.Payments.API.Infrastructure.Repositories;

public interface IInvoiceRepository
{
    Task AddAsync(Invoice invoice);
}
