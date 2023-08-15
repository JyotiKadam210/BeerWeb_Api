using BeerWeb.Api.DataAccess.DatabaseContext;
using BeerWeb.Api.DataAccess.Interface;
using BeerWeb.Api.DataAccess.UnitOfWork;
using BeerWeb.Api.Middleware;
using BeerWeb.Api.Services;
using BeerWeb.Api.Services.AutoMapper;
using BeerWeb.Api.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<BeerStoreDbContext>(option => option.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("logs/BeerStore-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Auto Mapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

// Services
builder.Services.AddSingleton<Serilog.ILogger>(logger);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IBeerService, BeerService>();
builder.Services.AddScoped<IBreweryService, BreweryService>();
builder.Services.AddScoped<IBarService, BarService>();
builder.Services.AddScoped<IBreweryBeersService, BreweryBeersService>();
builder.Services.AddScoped<IBarBeersService, BarBeerService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
