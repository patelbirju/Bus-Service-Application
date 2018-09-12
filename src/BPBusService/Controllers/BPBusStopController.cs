using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BPBusService.Models;

namespace BPBusService.Controllers
{
    /*  Birju Patel
     *  Student ID : 7120967
     *  9/20/2016
     * 
     *  BPBusStopController is a controller which manages the Index, Create, Edit, Delete and Details views 
     *  of the Bus Stops for the Bus Service apllication
    */
    public class BPBusStopController : Controller
    {
        private readonly BusServiceContext _context;
        public BPBusStopController(BusServiceContext context)
        {
            _context = context;    
        }

        // GET: BPBusStop
       //  The Index view gets called when the Bus Stops link is clicked and displays the Bus Stops with the 'GoingDowntown','Location' and 'LocationHash' fields
        public async Task<IActionResult> Index()
        {
            return View(await _context.BusStop.ToListAsync());
        }

        // GET: BPBusStop/Details/5
        // The Detials view gets called when the detials of a particular stop is opened 
        // It displays the BusStopNumber, GoingDowntown, Location and LocationHash
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busStop = await _context.BusStop.SingleOrDefaultAsync(m => m.BusStopNumber == id);
            if (busStop == null)
            {
                return NotFound();
            }

            return View(busStop);
        }

        // GET: BPBusStop/Create
        // The Create view gets called when a new Bus Stop has to be created and allows the Bu Stop details to be entered
        public IActionResult Create()
        {
            return View();
        }

        // POST: BPBusStop/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        // This Create view gets called when detials about the new Bus Stop have been entered and the save button is clicked
        // The new Bus Stop and the details are added to the databse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BusStopNumber,GoingDowntown,Location,LocationHash")] BusStop busStop)
        {
            if (ModelState.IsValid)
            {
                busStop.LocationHash = getHashValue(busStop.Location);
                
                _context.Add(busStop);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(busStop);
        }

        // GET: BPBusStop/Edit/5
        // The Edit view gets called when the details of a particular Bus Stop need to be changed

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busStop = await _context.BusStop.SingleOrDefaultAsync(m => m.BusStopNumber == id);
            if (busStop == null)
            {
                return NotFound();
            }
            return View(busStop);
        }

        // POST: BPBusStop/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

       // This Edit view gets called when the details of a bus stop have been edited and needs to be saved to the database
       // The details are passed through the paramater
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BusStopNumber,GoingDowntown,Location,LocationHash")] BusStop busStop)
        {
            if (id != busStop.BusStopNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    busStop.LocationHash = getHashValue(busStop.Location);
                    _context.Update(busStop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusStopExists(busStop.BusStopNumber))
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
            return View(busStop);
        }

        // GET: BPBusStop/Delete/5
        // This Delete view takes the Bus Stop number as the parameter and pulls up the details  awaiting for confirmation to delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busStop = await _context.BusStop.SingleOrDefaultAsync(m => m.BusStopNumber == id);
            if (busStop == null)
            {
                return NotFound();
            }

            return View(busStop);
        }

        // POST: BPBusStop/Delete/5
        // This Delete view removes the record with the id passed through the parameter from the database and returns to the Index view
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var busStop = await _context.BusStop.SingleOrDefaultAsync(m => m.BusStopNumber == id);
            _context.BusStop.Remove(busStop);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //show all routes using the stop given (if any)
        public  IActionResult RouteSelector(int busStopNumber = 0)
        {
            if (busStopNumber == 0)
            {
                TempData["message"] = "Please select a bus stop";
                return RedirectToAction("Index");
            }
            //ViewBag.busStopNumber = busStopNumber;
            var busStop = _context.BusStop.Where(x => x.BusStopNumber == busStopNumber).Include(x => x.Location);

            Response.Cookies.Append("busStopNumber", busStopNumber.ToString());
            
            

            var routeStops = _context.RouteStop.Where(x => x.BusStopNumber == busStopNumber).Include(x => x.BusRouteCodeNavigation);
            

            // If there are no routes using that stop, display a message to that effect
            if(routeStops.Count() <= 0)
            {
                TempData["message"] = "No routes uses stop " + busStopNumber;
                return RedirectToAction("Index");
            }

            //If only one route uses the stop, pass the RouteStopId to BPRouteSchedule controller's RouteStopSchedule Action
            if(routeStops.Count() == 1)
            {
                int routeStopId = routeStops.Select(x => x.RouteStopId).Single();
                return RedirectToAction("RouteStopSchedule", "BPRouteSchedule", new { routeStopId = routeStopId });
            }
            else
            {
                ViewBag.routeStopId = new SelectList(routeStops.Select(x => new { x.RouteStopId, x.BusRouteCodeNavigation.RouteName }).OrderBy(x => x.RouteName).ToList(), "routeStopId", "routeName");
            }

            return View(routeStops);
        }

        private bool BusStopExists(int id)
        {
            return _context.BusStop.Any(e => e.BusStopNumber == id);
        }

        // Hash Function that generates the hash key by adding up the byte value of each character in the string passed
        private int getHashValue(string location)
        {
          int hashValue = 0;
          for (int i=0; i < location.Length; i++)
            {
                hashValue += Convert.ToInt32(location.ElementAt(i));
            }
            return hashValue;
        }
    }
}
