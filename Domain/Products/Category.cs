using Flunt.Validations;

namespace IWantApp.Domain.Products;

public class Category : Entity
{
    public Category(string name)
    {
        var contract = new Contract<Category>().IsNotNullOrEmpty(name, "Name", "Nome é obrigatório");
        AddNotifications(contract);
        Name = name;
        Active = true;
    }
    public string Name { get; set; }
    public bool Active { get; set; }
}
