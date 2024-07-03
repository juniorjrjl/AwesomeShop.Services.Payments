using AwesomeShop.Services.Payments.API.Core.Entities;
using MongoDB.Driver;

namespace AwesomeShop.Services.Payments.API.Infrastructure.Repositories;

public class InvoiceRepository(IMongoDatabase database) : IInvoiceRepository
{
    private readonly IMongoCollection<Invoice> _collection = database.GetCollection<Invoice>("invoices");
    
    public async Task AddAsync(Invoice invoice) => await _collection.InsertOneAsync(invoice);

}
