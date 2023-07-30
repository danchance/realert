using Amazon.SimpleEmail;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Realert.Data;
using Realert.Interfaces;
using Realert.Models;
using Realert.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RealertContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RealertContext") ?? throw new InvalidOperationException("Connection string 'RealertContext' not found.")));

builder.Services.AddAWSService<IAmazonSimpleEmailService>().AddTransient<EmailService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IPriceAlertService, PriceAlertService>();
builder.Services.AddScoped<INewPropertyAlertService, NewPropertyAlertService>();
builder.Services.AddScoped<IEmailService, EmailService>();

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
