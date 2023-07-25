using Microsoft.AspNetCore.Mvc;
using Realert.Scrapers;

namespace Realert.Controllers
{
    public class HomeController : Controller
    {

        public async Task<IActionResult> Index()
        {
            string url = "https://www.purplebricks.co.uk/Api/Propertylisting/1460288";
            try
            {
                PropertyListingWebScraper propertyScraper = await PropertyListingWebScraper.InitializeAsync(url, Models.TargetSite.Purplebricks);
                Console.WriteLine(propertyScraper.PropertyName);
                Console.WriteLine(propertyScraper.PropertyPrice);


            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View();
        }

    }
}