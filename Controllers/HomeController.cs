using Microsoft.AspNetCore.Mvc;
using Realert.Scrapers;
using Realert.Models;

namespace Realert.Controllers
{
    public class HomeController : Controller
    {

        public async Task<IActionResult> Index()
        {
            string url = "https://www.purplebricks.co.uk/Api/Propertylisting/1460288";
            string url2 = "https://www.rightmove.co.uk/property-for-sale/find.html?searchType=SALE&locationIdentifier=OUTCODE%5E2593&insId=1&radius=0.0&minPrice=150000&maxPrice=200000&minBedrooms=&maxBedrooms=&displayPropertyType=houses&maxDaysSinceAdded=&_includeSSTC=on&sortByPriceDescending=&primaryDisplayPropertyType=&secondaryDisplayPropertyType=&oldDisplayPropertyType=&oldPrimaryDisplayPropertyType=&newHome=&auction=false";
            string url3 = "https://www.purplebricks.co.uk/search/property-for-sale/telford?page=1&sortBy=2&location=tf2&searchRadius=10&searchType=ForSale&soldOrLet=false&latitude=52.7102563&longitude=-2.419879&betasearch=true";
            try
            {
                /*PropertyListingWebScraper propertyScraper = await PropertyListingWebScraper.InitializeAsync(url, TargetSite.Purplebricks);
                Console.WriteLine(propertyScraper.PropertyName);
                Console.WriteLine(propertyScraper.PropertyPrice);*/
                NewListingsWebScraper scraper = await NewListingsWebScraper.InitializeAsync(url3, TargetSite.Purplebricks);
                Console.WriteLine(scraper.ResultCount);


            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View();
        }

    }
}