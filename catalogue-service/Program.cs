using System.Reflection;
using CatalogueService.Model;
using CatalogueService.Model.Database;
using CatalogueService.Model.Database.Types;
using DynamicQL.Extensions;
using DynamicQL.Model.Types;
using GraphiQl;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using CatalogueService.Model.Database.Enums;
using Utility.Api.Middlewares;
using Utility.EFCore;
using Utility.Other.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CatalogueContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("salamdo_catalogue"))
        .UseSnakeCaseNamingConvention()
    );
builder.Services.AddTransient<DbContext>(sp => sp.GetRequiredService<CatalogueContext>());

builder.Services.AddDynamicGraphQL(options =>
{
    options.Assemblies.Add(Assembly.GetAssembly(typeof(Brand))!);
    options.MaxQueryDepth = 2;
    options.MinimumExecutionTime = 0;
    options.Endpoint = builder.Configuration.GetValue<string>("GraphQL:Api:Endpoint");
});

builder.Services.AddGraphiQl(options =>
{
    options.GraphiQlPath = builder.Configuration.GetValue<string>("GraphQL:UI:Endpoint") ?? "/ui/development/graphiql";
    options.GraphQlApiPath = builder.Configuration.GetValue<string>("GraphQL:UI:ApiEndpoint");
});

builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddDefaultPolicy(b =>
        {
            b.WithMethods("*");
            b.WithHeaders("*");
            b.WithOrigins("*");
        });
    }
});
builder.Services.AddSingleton<EnumerationGraphType<SortBy>>();
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

// https redirection throws a cors error in the client app in development
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

if (app.Configuration.GetValue<bool>("GraphQL:UI:IsActive"))
{
    app.UseGraphiQl();
}

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseDynamicGraphQL();

var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
await dbContext.Database.MigrateAsync();

if(app.Environment.IsDevelopment() && (await dbContext.Set<Product>().CountAsync()) == 0)
{
    var serializerOptions = new JsonSerializerOptions()
    {
        PropertyNameCaseInsensitive = true
    };
    var storagePath = Path.Join(Environment.CurrentDirectory, app.Configuration.GetValue<string>("Storage"));

    var dbImages = JsonSerializer.Deserialize<DatabaseImage[]>(
        File.ReadAllText(Path.Join(storagePath, "database-images.json")), 
        serializerOptions
        );

    foreach (var dbImage in dbImages)
    {
        foreach (var dir in Directory.GetDirectories(Path.Join(storagePath, "images")))
        {
            var filePath = Path.Join(dir, dbImage.FileName);
            if (File.Exists(filePath))
            {
                dbImage.Data = File.ReadAllBytes(filePath);
                break;
            }
        }
    }

    var images = JsonSerializer.Deserialize<Image[]>(
        File.ReadAllText(Path.Join(storagePath, "images.json")),
        serializerOptions
        );

    foreach (var image in images)
    {
        var dbImage = dbImages.First(x => x.Id == image.DatabaseImageId);
        image.Hash = dbImage.Data.GetChecksum(Utility.Other.Enums.HashAlgorithm.Md5);
    }


    var brands = JsonSerializer.Deserialize<Brand[]>(
        File.ReadAllText(Path.Join(storagePath, "brands.json")),
        serializerOptions
        );

    var categories = JsonSerializer.Deserialize<ProductCategory[]>(
        File.ReadAllText(Path.Join(storagePath, "product-category.json")),
        serializerOptions
        );

    var currencies = JsonSerializer.Deserialize<Currency[]>(
        File.ReadAllText(Path.Join(storagePath, "currency.json")),
        serializerOptions
        );

    var products = JsonSerializer.Deserialize<Product[]>(
        File.ReadAllText(Path.Join(storagePath, "products.json")),
        serializerOptions
        );

    var productImages = JsonSerializer.Deserialize<ProductImage[]>(
        File.ReadAllText(Path.Join(storagePath, "product-images.json")),
        serializerOptions
        );

    var transaction = await dbContext.Database.BeginTransactionAsync();
    await dbContext.AddRangeAsync(dbImages);
    await dbContext.AddRangeAsync(images);
    await dbContext.AddRangeAsync(brands);
    await dbContext.AddRangeAsync(categories);
    await dbContext.AddRangeAsync(currencies);
    await dbContext.AddRangeAsync(products);
    await dbContext.AddRangeAsync(productImages);
    await dbContext.SaveChangesAsync();
    await transaction.CommitAsync();
}

app.Run();
