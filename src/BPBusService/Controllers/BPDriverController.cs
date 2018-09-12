using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BPBusService.Models;
/*  Birju Patel
     *  Student ID : 7120967
     *  11/15/2016
     * 
     *  BPDriverController is a controller which manages the Index, Create, Edit, Delete and Details views 
     *  of the Drivers' details for the Bus Service apllication
    */
namespace BPBusService.Controllers
{
    public class BPDriverController : Controller
    {
        private readonly BusServiceContext _context;

        public BPDriverController(BusServiceContext context)
        {
            _context = context; 
        }



        // GET: BPDriver
        public async Task<IActionResult> Index()
        {
            var busServiceContext = _context.Driver.Include(d => d.ProvinceCodeNavigation).OrderBy(d => d.FullName);
            return View(await busServiceContext.ToListAsync());
        }

        // GET: BPDriver/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Driver.SingleOrDefaultAsync(m => m.DriverId == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // GET: BPDriver/Create
        public IActionResult Create()
        {
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode");
            return View();
        }

        // POST: BPDriver/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DriverId,City,DateHired,FirstName,FullName,HomePhone,LastName,PostalCode,ProvinceCode,Street,WorkPhone")] Driver driver)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(driver);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Driver successfully added";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error adding a new driver: {ex.GetBaseException().Message}");
                }
                
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", driver.ProvinceCode);
            return View(driver);
        }

        // GET: BPDriver/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Driver.SingleOrDefaultAsync(m => m.DriverId == id);
            if (driver == null)
            {
                return NotFound();
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province.OrderBy(d => d.Name), "ProvinceCode", "ProvinceCode", driver.ProvinceCode);
            TempData["fullName"] = driver.FullName;
            return View(driver);
        }

        // POST: BPDriver/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DriverId,City,DateHired,FirstName,FullName,HomePhone,LastName,PostalCode,ProvinceCode,Street,WorkPhone")] Driver driver)
        {
            if (id != driver.DriverId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(driver);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Driver details successfully changed";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!DriverExists(driver.DriverId))
                    {
                        //return NotFound();
                        ModelState.AddModelError("", $"Error editing the details of driver: {ex.GetBaseException().Message}");
                    }
                    else
                    { 
                        throw;
                    }
                }
                //TempData["successMessage"] = "Successfully changed details";
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "ProvinceCode", driver.ProvinceCode);
            return View(driver);
        }

        // GET: BPDriver/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var driver = await _context.Driver.SingleOrDefaultAsync(m => m.DriverId == id);
            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // POST: BPDriver/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var driver = await _context.Driver.SingleOrDefaultAsync(m => m.DriverId == id);
                _context.Driver.Remove(driver);
                await _context.SaveChangesAsync();
                TempData["message"] = "Driver successfully deleted";
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                TempData["message"] = ex.InnerException.Message;
            }
            return View("Index");
        }

        private bool DriverExists(int id)
        {
            return _context.Driver.Any(e => e.DriverId == id);
        }
    }
}
