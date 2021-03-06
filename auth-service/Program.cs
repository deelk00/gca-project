using AuthService.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AuthenticationContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("AuthenticationDatabase"))
        .UseSnakeCaseNamingConvention()
);

builder.Services.AddTransient<DbContext, AuthenticationContext>(sp => sp.GetRequiredService<AuthenticationContext>());


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


await app.Services.CreateScope()
    .ServiceProvider
    .GetRequiredService<DbContext>()
    .Database.MigrateAsync();

app.Run();
