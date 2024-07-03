namespace AwesomeShop.Services.Payments.API.Subscribers.Events;

public record PaymentInfo(string CardNumber, string FullName, string ExpirationDate, string Cvv);
