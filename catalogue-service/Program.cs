using CatalogueService.Model;
using CatalogueService.Model.Database;
using Microsoft.EntityFrameworkCore;
using Utility.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<CatalogueContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("salamdo_catalogue"))
        .UseSnakeCaseNamingConvention()
    );

builder.Services.AddTransient<DbContext>(sp => sp.GetRequiredService<CatalogueContext>());
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.AddDefaultPolicy(b =>
        {
            b.AllowAnyMethod();
            b.AllowAnyHeader();
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
app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
await dbContext.Database.MigrateAsync();

app.Run();
