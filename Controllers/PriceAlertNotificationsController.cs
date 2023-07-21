using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Realert.Data;
using Realert.Models;

namespace Realert.Controllers
{
    public class PriceAlertNotificationsController : Controller
    {
        private readonly RealertContext _context;

        public PriceAlertNotificationsController(RealertContext context)
        {
            _context = context;
        }

        // GET: PriceAlertNotifications
        public async Task<IActionResult> Index()
        {
              return _context.PriceAlertNotification != null ? 
                          View(await _context.PriceAlertNotification.ToListAsync()) :
                          Problem("Entity set 'RealertContext.PriceAlertNotification'  is null.");
        }

        // GET: PriceAlertNotifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PriceAlertNotification == null)
            {
                return NotFound();
            }

            var priceAlertNotification = await _context.PriceAlertNotification
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceAlertNotification == null)
            {
                return NotFound();
            }

            return View(priceAlertNotification);
        }

        // GET: PriceAlertNotifications/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PriceAlertNotifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,PhoneNumber,ListingLink,TargetSite,PriceThreshold,NotifyOnPriceIncrease,NotifyOnPropertyDelist,NotificationType,Note")] PriceAlertNotification priceAlertNotification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(priceAlertNotification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(priceAlertNotification);
        }

        // GET: PriceAlertNotifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PriceAlertNotification == null)
            {
                return NotFound();
            }

            var priceAlertNotification = await _context.PriceAlertNotification.FindAsync(id);
            if (priceAlertNotification == null)
            {
                return NotFound();
            }
            return View(priceAlertNotification);
        }

        // POST: PriceAlertNotifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,PhoneNumber,ListingLink,TargetSite,PriceThreshold,NotifyOnPriceIncrease,NotifyOnPropertyDelist,NotificationType,Note")] PriceAlertNotification priceAlertNotification)
        {
            if (id != priceAlertNotification.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(priceAlertNotification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceAlertNotificationExists(priceAlertNotification.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(priceAlertNotification);
        }

        // GET: PriceAlertNotifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PriceAlertNotification == null)
            {
                return NotFound();
            }

            var priceAlertNotification = await _context.PriceAlertNotification
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceAlertNotification == null)
            {
                return NotFound();
            }

            return View(priceAlertNotification);
        }

        // POST: PriceAlertNotifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PriceAlertNotification == null)
            {
                return Problem("Entity set 'RealertContext.PriceAlertNotification'  is null.");
            }
            var priceAlertNotification = await _context.PriceAlertNotification.FindAsync(id);
            if (priceAlertNotification != null)
            {
                _context.PriceAlertNotification.Remove(priceAlertNotification);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PriceAlertNotificationExists(int id)
        {
          return (_context.PriceAlertNotification?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
