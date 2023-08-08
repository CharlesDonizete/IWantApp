using IWantApp.Domain.Users;

namespace IWantApp.Endpoints.Clients;

public class OrderPost
{
    public static string Template => "/orders";
    public static string[] Methods => new string[] { HttpMethod.Post.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "CpfPolicy")]
    public static async Task<IResult> Action(OrderRequest orderRequest, HttpContext http, ApplicationDbContext context)
    {   
        var clientId = http.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var clientName = http.User.Claims.First(c => c.Type == "Name").Value;

        //if (orderRequest.ProductsIds == null || !orderRequest.ProductsIds.Any())
        //    return Results.BadRequest("Produto é obrigatório para pedido.");

        //if(string.IsNullOrEmpty(orderRequest.DelivceryAddress))
        //    return Results.BadRequest("Endereço de entrega é obrigatório.");

        List<Product> productsFound = null;
        if (orderRequest.ProductsIds != null && orderRequest.ProductsIds.Any())
             productsFound = context.Products.Where(p => orderRequest.ProductsIds.Contains(p.Id)).ToList();               
        
        var order = new Order(clientId, clientName, productsFound, orderRequest.DelivceryAddress);
        if (!order.IsValid)
            return Results.ValidationProblem(order.Notifications.ConvertToProblemDetails());

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        return Results.Created($"/clients/{order.Id}", order.Id);
    }
}
