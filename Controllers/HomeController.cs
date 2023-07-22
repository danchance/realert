using Microsoft.AspNetCore.Mvc;
using Realert.Models;
using System.Diagnostics;

namespace Realert.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email")] NewPropertyAlertSetupViewModel priceAlert)
        {
            if (ModelState.IsValid)
            {

            }
            else
            {
                return View("Index", priceAlert);
            }
            /*if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }*/
            return View("Index", priceAlert);
        }

    }
}