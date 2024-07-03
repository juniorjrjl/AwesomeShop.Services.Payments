using AwesomeShop.Services.Payments.API.Infrastructure.DTOs;

namespace AwesomeShop.Services.Payments.API.Infrastructure;

public interface IPaymentGatewayService
{
    Task<bool> Process(CreditCardInfo info);
}