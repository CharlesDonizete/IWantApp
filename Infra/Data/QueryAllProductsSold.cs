namespace IWantApp.Infra.Data;

public class QueryAllProductsSold
{
    private readonly IConfiguration configuration;

    public QueryAllProductsSold(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<IEnumerable<ProductSoldReportResponse>> Execute()
    {
        var db = new SqlConnection(configuration["ConnectionString:IWantDb"]);
        var query = @"
            select
                p.Id,
                p.Name,
                count(*) Amount
            from 
                Orders o
                inner join OrderProducts op on o.Id = op.OrdersId
                inner join Products p on p.Id = op.ProductsId
            group by
                p.id, p.Name
            order by
                p.Id, p.Name        
        ";

        //context.Database.GetDbConnection().Query<EmployeeResponse>(query, new { page, rows })

        return await db.QueryAsync<ProductSoldReportResponse>(query);
    }
}
