using System.Reflection;
using CatalogueService.Model;
using CatalogueService.Model.Database;
using CatalogueService.Model.Database.Types;
using DynamicQL.Extensions;
using DynamicQL.Model.Types;
using GraphiQl;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using Utility.Api.Middlewares;

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

app.Run();
