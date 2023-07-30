using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Realert.Data;
using Realert.Interfaces;
using Realert.Models;
using Realert.Services;

namespace Realert.Controllers
{
    public class HomeController : Controller
    {

        private readonly RealertContext _context;
        private readonly INewPropertyAlertService _service;

        public HomeController(RealertContext context, INewPropertyAlertService service)
        {
            _service = service;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //var priceAlert = await _context.PriceAlertNotification.Include("Property").FirstOrDefaultAsync(n => n.Id == 34);

            //PriceAlertService priceAlertService = new(_context);
            //var msgId = await priceAlertService.SendPriceAlert(priceAlert, priceAlert.Property, 500000);
            // await priceAlertService.PerformScanAsync();

            await _service.PerformScanAsync();


            //Console.WriteLine(msgId);

            return View();
        }

    }
}