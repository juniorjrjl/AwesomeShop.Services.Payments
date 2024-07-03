namespace AwesomeShop.Services.Payments.API.Core.Entities;

public class Invoice(decimal totalPrice, Guid orderId, string cardNumber)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public decimal TotalPrice { get; private set; } = totalPrice;
    public Guid OrderId { get; private set; } = orderId;
    public string CardNumber { get; private set; } = "**-" + cardNumber[^4..];
    public DateTime PaidAt { get; private set; } = DateTime.Now;

}