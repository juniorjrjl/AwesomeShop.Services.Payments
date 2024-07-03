namespace AwesomeShop.Services.Payments.API.Infrastructure.DTOs;

public record CreditCardInfo(string CardNumber, string FullName, string ExpirationDate, string Cvv);
