using System.Reflection;
using CartService.Database;
using CartService.Database.Model;
using DynamicQL.Extensions;
using GraphiQl;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CartContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("salamdo_catalogue"))
        .UseSnakeCaseNamingConvention()
);
builder.Services.AddTransient<DbContext>(sp => sp.GetRequiredService<CartContext>());

builder.Services.AddDynamicGraphQL(options =>
{
    options.Assemblies.Add(Assembly.GetAssembly(typeof(ShoppingCart))!);
    options.MaxQueryDepth = 2;
    options.MinimumExecutionTime = 0;
    options.Endpoint = builder.Configuration.GetValue<string>("GraphQL:Api:Endpoint");
});

builder.Services.AddGraphiQl(options =>
{
    options.GraphiQlPath = builder.Configuration.GetValue<string>("GraphQL:UI:Endpoint") ?? "/ui/development/graphiql";
    options.GraphQlApiPath = builder.Configuration.GetValue<string>("GraphQL:UI:ApiEndpoint");
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseDynamicGraphQL();

var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
await dbContext.Database.MigrateAsync();

app.Run();
