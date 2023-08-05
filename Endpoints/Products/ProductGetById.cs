using Microsoft.AspNetCore.Http;

namespace IWantApp.Endpoints.Products;

public class ProductGetById
{
    public static string Template => "/products/{id:guid}";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "Employee005Policy")]
    public static async Task<IResult> Action([FromRoute] Guid id, ApplicationDbContext context)
    {
        var product = await context.Products.Include(p => p.Category).Where(p => p.Id == id).SingleOrDefaultAsync();
        if (product != null)
        {
            var results = new ProductResponse(product.Id, product.Name, product.Category.Name, product.Description, product.HasStock, product.Price, product.Active);
            return Results.Ok(results);
        }
        return Results.BadRequest();
    }
}
