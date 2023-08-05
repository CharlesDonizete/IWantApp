namespace IWantApp.Domain.Orders;

public class Order : Entity
{
    public string ClientId { get; private set; }
    public List<Product> Products { get; private set; }
    public decimal Total { get; private set; }
    public string DeliveryAddress { get; private set; }

    public Order() { }

    public Order(string clientId, string clientName, List<Product> products, decimal total, string deliveryAddress)
    {
        ClientId = clientId;
        Products = products;
        Total = total;
        DeliveryAddress = deliveryAddress;
        CreatedBy = clientName;
        EditedBy = clientName;
        CreatedOn = DateTime.UtcNow;
        EditedOn = DateTime.UtcNow;

        Total = products.Sum(p => p.Price);

        Validate();
    }

    private void Validate()
    {
        var contract = new Contract<Order>()
            .IsNotNull(ClientId, "Client")
            .IsNotNull(Products, "Products");

        AddNotifications(contract);
    }
}
