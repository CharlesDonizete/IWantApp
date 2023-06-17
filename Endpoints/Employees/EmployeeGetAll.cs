using IWantApp.Infra.Data;
using Microsoft.AspNetCore.Authorization;

namespace IWantApp.Endpoints.Employees;

public class EmployeeGetAll
{
    public static string Template => "/employees";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "Employee005Policy")]
    public static IResult Action(int? page, int? rows, QueryAllUsersWithClaimName query)
    {
        if (page == null)
            return Results.BadRequest("Informe a página!");

        if (rows == null)
            return Results.BadRequest("Informe a quantidade de linhas!");

        if (rows > 10)
            return Results.BadRequest("Limite máximo de linhas permitido é 10.");

        return Results.Ok(query.Execute(page.Value, rows.Value));
    }    
}
