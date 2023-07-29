using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Realert.Data;
using Realert.Models;
using Realert.Services;

namespace Realert.Controllers
{
    public class HomeController : Controller
    {

        private readonly RealertContext _context;

        public HomeController(RealertContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var priceAlert = await _context.PriceAlertNotification.Include("Property").FirstOrDefaultAsync(n => n.Id == 34);

            PriceAlertService priceAlertService = new(_context);
            //var msgId = await priceAlertService.SendPriceAlert(priceAlert, priceAlert.Property, 500000);
            //await priceAlertService.SendDelistAlert(priceAlert, priceAlert.Property);


            //Console.WriteLine(msgId);

            return View();
        }

    }
}