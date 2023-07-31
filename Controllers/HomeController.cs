using Microsoft.AspNetCore.Mvc;
using Realert.Data;
using Realert.Interfaces;

namespace Realert.Controllers
{
    public class HomeController : Controller
    {
        private readonly RealertContext context;
        private readonly INewPropertyAlertService service;

        public HomeController(RealertContext context, INewPropertyAlertService service)
        {
            this.service = service;
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            // var priceAlert = await _context.PriceAlertNotification.Include("Property").FirstOrDefaultAsync(n => n.Id == 34);

            /*PriceAlertService priceAlertService = new(_context);
             var msgId = await priceAlertService.SendPriceAlert(priceAlert, priceAlert.Property, 500000);
             await priceAlertService.PerformScanAsync(); */

            await this.service.PerformScanAsync();

            // Console.WriteLine(msgId);
            return this.View();
        }
    }
}