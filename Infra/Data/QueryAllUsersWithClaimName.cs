﻿namespace IWantApp.Infra.Data;

public class QueryAllUsersWithClaimName
{
    private readonly IConfiguration configuration;

    public QueryAllUsersWithClaimName(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<IEnumerable<EmployeeResponse>> Execute(int page, int rows)
    {
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

        //context.Database.GetDbConnection().Query<EmployeeResponse>(query, new { page, rows })

        return await db.QueryAsync<EmployeeResponse>(query, new { page, rows });
    }
}
