using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BPBusService.Models;
using Microsoft.AspNetCore.Authorization;

namespace BPBusService.Controllers
{
    /*
     *  Birju Patel
     *  Student ID : 7120967
     *  9/20/2016
     * 
     *  BPBusRouteController is a controller which manages the Index, Create, Edit, Delete and Details views 
     *  of the Bus Routes for the Bus Service apllication
    */
    [Authorize]
    public class BPBusRouteController : Controller
    {
        private readonly BusServiceContext _context;

        public BPBusRouteController(BusServiceContext context)
        {
            _context = context;    
        }

        // Index view gets called when the Bus Routes page is loaded and lists all the Bus Routes with their Bus Route Codes.
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {        
            return View(await _context.BusRoute.OrderBy(x =>Convert.ToInt32(x.BusRouteCode)).ToListAsync());
        }

        // GET: BPBusRoute/Details/5
        // Details view gets called when a particular Bus Route's details are requested and it displays the Route Name and the Route Code
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busRoute = await _context.BusRoute.SingleOrDefaultAsync(m => m.BusRouteCode == id);
            if (busRoute == null)
            {
                return NotFound();
            }

            return View(busRoute);
        }

        // GET: BPBusRoute/Create
        // Create view gets called when a new Bus Route needs to be created and allows the details of the Bus Route to be entered
        public IActionResult Create()
        {
            return View();
        }

        // POST: BPBusRoute/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // This Create view gets called when the details of the new Bus Route have been enetered
        // The new record is added to the BusRoute table
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BusRouteCode,RouteName")] BusRoute busRoute)
        {
            if (ModelState.IsValid)
            {
                _context.Add(busRoute);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(busRoute);
        }

        // GET: BPBusRoute/Edit/5
        // This Edit view gets called when the details of a Bus Route need to be edited
        // It takes the id as a paramter and checks for the details and displays them to be edited 
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busRoute = await _context.BusRoute.SingleOrDefaultAsync(m => m.BusRouteCode == id);
            if (busRoute == null)
            {
                return NotFound();
            }
            return View(busRoute);
        }

        // POST: BPBusRoute/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

       // This Edit view gets called when the details of the Bus Route have been edited and the save button is clicked
       // The detials are uodated in the Bus Route table
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BusRouteCode,RouteName")] BusRoute busRoute)
        {
            if (id != busRoute.BusRouteCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(busRoute);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusRouteExists(busRoute.BusRouteCode))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(busRoute);
        }

        // GET: BPBusRoute/Delete/5
        // This Delete view gets called when a record needs to be deleted form the Bus Route table
        // The id is passed as a paramter and the details of the Bus Route with that id are pulled up
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busRoute = await _context.BusRoute.SingleOrDefaultAsync(m => m.BusRouteCode == id);
            if (busRoute == null)
            {
                return NotFound();
            }

            return View(busRoute);
        }

        // POST: BPBusRoute/Delete/5
        // This Delete view gets called when the user confirms the Bus Route to be deleted
        // The recored is removed from the Bus Route table and the changes are saved
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var busRoute = await _context.BusRoute.SingleOrDefaultAsync(m => m.BusRouteCode == id);
            _context.BusRoute.Remove(busRoute);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool BusRouteExists(string id)
        {
            return _context.BusRoute.Any(e => e.BusRouteCode == id);
        }
    }
}
