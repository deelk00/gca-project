using CheckoutService.Model.Database;
using CheckoutService.Services;
using DynamicQL.Extensions;
using GraphiQl;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CheckoutContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("salamdo_checkout"))
        .UseSnakeCaseNamingConvention()
);
builder.Services.AddTransient<DbContext>(sp => sp.GetRequiredService<CheckoutContext>());

builder.Services.AddSingleton<RemoteCartService>();

builder.Services.AddHttpClient();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseCors();

app.UseAuthorization();

app.MapControllers();

var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
await dbContext.Database.MigrateAsync();

app.Run();