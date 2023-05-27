using Dapper;
using IWantApp.Infra.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IWantApp.Endpoints.Employees;

public class EmployeeGetAll
{
    public static string Template => "/employees";
    public static string[] Methods => new string[] { HttpMethod.Get.ToString() };
    public static Delegate Handle => Action;    

    public static IResult Action(int? page, int? rows, IConfiguration configuration)
    {
        if (page == null)
            return Results.BadRequest("Informe a página!");

        if (rows == null)
            return Results.BadRequest("Informe a quantidade de linhas!");

        if (rows > 10)
            return Results.BadRequest("Limite máximo de linhas permitido é 10.");

        var db = new SqlConnection(configuration["ConnectionString:IWantDb"]);
        var query = @"
        select
            Email, ClaimValue as Name
        from 
            AspNetUsers 
            inner join  AspNetUserClaims 
            on AspNetUsers.Id = AspNetUserClaims.UserId and ClaimType = 'Name'
        order by
            Name
        OFFSET(@page -1) * @rows ROWS FETCH NEXT @rows ROWS ONLY
        ";

        var employees = db.Query<EmployeeResponse>(query, new { page, rows });

        return Results.Ok(employees);
    }

    public static IResult Action1(int? page, int? rows, ApplicationDbContext context)
    {
        if (page == null)
            return Results.BadRequest("Informe a página!");

        if (rows == null)
            return Results.BadRequest("Informe a quantidade de linhas!");

        if (rows > 10)
            return Results.BadRequest("Limite máximo de linhas permitido é 10.");
        
        var query = @"
        select
            Email, ClaimValue as Name
        from 
            AspNetUsers 
            inner join  AspNetUserClaims 
            on AspNetUsers.Id = AspNetUserClaims.UserId and ClaimType = 'Name'
        order by
            Name
        OFFSET(@page -1) * @rows ROWS FETCH NEXT @rows ROWS ONLY
        ";

        var employees = context.Database.GetDbConnection().Query<EmployeeResponse>(query, new { page, rows });

        return Results.Ok(employees);
    }
}
