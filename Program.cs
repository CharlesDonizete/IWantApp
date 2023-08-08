
using IWantApp.Domain.Users;
using IWantApp.Endpoints.Scurity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["ConnectionString:IWantDb"]);
//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMysql(builder.Configuration["ConnectionString:IWantDb"]);

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => 
{ 
    options.Password.RequireNonAlphanumeric = false; 
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 3;
}).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<QueryAllUsersWithClaimName>();
builder.Services.AddScoped<UserCreator>();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
      .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
      .RequireAuthenticatedUser()
      .Build();

    options.AddPolicy("EmployeePolicy", p => p.RequireAuthenticatedUser().RequireClaim("EmployeeCode"));
    options.AddPolicy("CpfPolicy", p => p.RequireAuthenticatedUser().RequireClaim("Cpf"));
});

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtBearerTokenSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtBearerTokenSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtBearerTokenSettings:SecretKey"]))
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.WebHost.UseSerilog((context, configuration) => {
    configuration
        .WriteTo.Console()
        .WriteTo.MSSqlServer(
            connectionString: context.Configuration["ConnectionString:IWantDb"],
            sinkOptions: new MSSqlServerSinkOptions
            {
                AutoCreateSqlTable = true,
                TableName = "LogAPI"
            }
        );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapMethods(CategoryPost.Template, CategoryPost.Methods, CategoryPost.Handle);
app.MapMethods(CategoryGetAll.Template, CategoryGetAll.Methods, CategoryGetAll.Handle);
app.MapMethods(CategoryPut.Template, CategoryPut.Methods, CategoryPut.Handle);
app.MapMethods(EmployeePost.Template, EmployeePost.Methods, EmployeePost.Handle);
app.MapMethods(EmployeeGetAll.Template, EmployeeGetAll.Methods, EmployeeGetAll.Handle);
app.MapMethods(TokenPost.Template, TokenPost.Methods, TokenPost.Handle);
app.MapMethods(ProductPost.Template, ProductPost.Methods, ProductPost.Handle);
app.MapMethods(ProductGetAll.Template, ProductGetAll.Methods, ProductGetAll.Handle);
app.MapMethods(ProductGetById.Template, ProductGetAll.Methods, ProductGetById.Handle);
app.MapMethods(ProductGetShowcase.Template, ProductGetShowcase.Methods, ProductGetShowcase.Handle);
app.MapMethods(ClientPost.Template, ClientPost.Methods, ClientPost.Handle);
app.MapMethods(ClientGet.Template, ClientGet.Methods, ClientGet.Handle);
app.MapMethods(OrderPost.Template, OrderPost.Methods, OrderPost.Handle);

app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext http) =>
{
    var error = http.Features?.Get<IExceptionHandlerFeature>()?.Error;
    if (error != null)
        if (error.InnerException is SqlException)
            return Results.Problem(title: "Database out", statusCode: 500);
        else if (error.InnerException is JsonException)
            return Results.Problem(title: "Error to convert data to other type. See all the information sent", statusCode: 500);

    return Results.Problem(title: "An error ocurred", statusCode: 500);
});

app.Run();