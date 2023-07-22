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

    }
}