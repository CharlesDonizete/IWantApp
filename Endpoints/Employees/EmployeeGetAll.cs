﻿namespace IWantApp.Endpoints.Employees;

public class EmployeeGetAll
{
    public static string Template => "/employees";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;

    [Authorize(Policy = "EmployeePolicy")]
    public static async Task<IResult> Action(int? page, int? rows, QueryAllUsersWithClaimName query)
    {
        if (page == null)
            return Results.BadRequest("Informe a página!");

        if (rows == null)
            return Results.BadRequest("Informe a quantidade de linhas!");

        if (rows > 10)
            return Results.BadRequest("Limite máximo de linhas permitido é 10.");

        var result = await query.Execute(page.Value, rows.Value);

        return Results.Ok(result);
    }    
}
