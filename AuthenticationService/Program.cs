using AuthenticationService.Model.Database;
using AuthenticationService.Model.Database.Types;
using DynamicQL.Authentication.Jwt.Extensions;
using DynamicQL.Extensions;
using GraphiQl;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Utility.Api.Middlewares;
using Utility.Extensions;
using Utility.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AuthenticationContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("salamdo_authentication"))
        .UseSnakeCaseNamingConvention()
    );
builder.Services.AddTransient<DbContext>(sp => sp.GetRequiredService<AuthenticationContext>());

builder.AddRedis();

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

app.UseMiddleware<ValidateJwtMiddleware>();

app.MapControllers();

var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
await dbContext.Database.MigrateAsync();

app.Run();
