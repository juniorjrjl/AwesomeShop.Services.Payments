namespace AwesomeShop.Services.Payments.API.Subscribers.Events;

public record OrderCreated(Guid Id, decimal TotalPrice, PaymentInfo PaymentInfo, string FullName, string Email);
