using AwesomeShop.Services.Payments.API.Infrastructure.DTOs;

namespace AwesomeShop.Services.Payments.API.Infrastructure;

public class PaymentGatewayService : IPaymentGatewayService
{
    public Task<bool> Process(CreditCardInfo _) => Task.FromResult(true);
}
