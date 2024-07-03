namespace AwesomeShop.Services.Payments.API.Events;

public record PaymentAccepted(Guid Id, string FullName, string Email);
