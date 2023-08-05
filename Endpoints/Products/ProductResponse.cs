namespace IWantApp.Endpoints.Products;

public record ProductResponse(Guid Id, string Name, string CategoryName, string Description, bool HasStock, decimal price, bool Active);
