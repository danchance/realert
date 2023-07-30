using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Realert.Data;
using Realert.Interfaces;
using Realert.Models;
using Realert.Services;

var builder = WebApplication.CreateBuilder(args);
var host = Host.CreateDefaultBuilder().Build();

var optionsBuilder = builder.Services.AddDbContext<RealertContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RealertContext") ?? throw new InvalidOperationException("Connection string 'RealertContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IPriceAlertService, PriceAlertService>();
builder.Services.AddScoped<INewPropertyAlertService, NewPropertyAlertService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
