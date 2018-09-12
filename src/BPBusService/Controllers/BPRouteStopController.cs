using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using BPBusService.Models;

namespace BPBusService.Controllers
{
    /*  Birju Patel
     *  Student ID : 7120967
     *  9/20/2016
     * 
     *  BPRouteStop is a controller which manages the Index, Create, Edit, Delete and Details views 
     *  of the Bus Stops of all Routes which have stops for the Bus Service apllication
    */
    public class BPRouteStopController : Controller
    {
        private readonly BusServiceContext _context;

        public BPRouteStopController(BusServiceContext context)
        {
            _context = context;  
        }

        // GET: BPRouteStop
        // The Index view gets called when a route is selected with a busRouteCode passed as a query string or a cookie
        // It returns all the stops assosciated with that particular bus Route
        // Parameter: busRouteCode, routeName
        public IActionResult Index(string busRouteCode = "", string routeName="")
        {
            //HttpCookie cookie = new HttpCookie("busRouteCode");
            if (busRouteCode != "")
            {
                Response.Cookies.Append("busRouteCode", busRouteCode.ToString());
                Response.Cookies.Append("routeName", routeName.ToString());
                ViewBag.busRouteCode = busRouteCode;
            }
            else
            {
                if (Request.Cookies["busRouteCode"] != null)
                {
                    busRouteCode = Request.Cookies["busRouteCode"];
                    routeName = Request.Cookies["routeName"];
                }
                else
                {
                    TempData["message"] = "Please select a bus route to see its stops";
                    return RedirectToAction("Index", "BPBusRoute");
                }


            }
            var busRoutes = _context.RouteStop.Where((x => x.BusRouteCode == busRouteCode)).Include(x => x.BusRouteCodeNavigation).Include(x => x.BusStopNumberNavigation).OrderBy(x => x.OffsetMinutes);
            
            //return View(await _context.BusRoute.ToListAsync());
            return View(busRoutes.ToList());

        }

        // GET: BPRouteStop/Details/5
        // This view gets called when the details of a particular stop of a bus Route are requested
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeStop = await _context.RouteStop.SingleOrDefaultAsync(m => m.RouteStopId == id);
            if (routeStop == null)
            {
                return NotFound();
            }

            return View(routeStop);
        }

        // GET: BPRouteStop/Create
        // The Create view gets called when a new stop for a route is added
        // The view provides a dropdown list of stop names for the user to select
        public IActionResult Create()
        {

            // Creating drop downs for Bus Stop Information when a new routeStop is added 

            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "RouteName");
            var BusStopNumber = _context.BusStop.OrderBy(a => a.Location).Select(x => new { Text = x.Location + "  " + x.GoingDowntown, Value = x.BusStopNumber }).ToList();

            ViewBag.BusStopNumber = new SelectList(BusStopNumber, "Value", "Text");

            return View();
        }

        // POST: BPRouteStop/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        // This Create view gets called when the details of a new stop added to a bus Route are being saved to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RouteStopId,BusRouteCode,BusStopNumber,OffsetMinutes")] RouteStop routeStop)
        {
            if (ModelState.IsValid)
            {
                _context.Add(routeStop);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "RouteName", routeStop.BusRouteCode);
            var BusStopNumber = _context.BusStop.OrderBy(a => a.Location).Select(x => new { Text = x.Location + "  " + x.GoingDowntown, Value = x.BusStopNumber }).ToList();

            ViewBag.BusStopNumber = new SelectList(BusStopNumber, "Value", "Text");

            return View(routeStop);
        }

        // GET: BPRouteStop/Edit/5
        // The Edit view gets called when the details of a stop of a particular bus Route are to be edited
        // Parameter: busRouteCode as an int
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeStop = await _context.RouteStop.SingleOrDefaultAsync(m => m.RouteStopId == id);
            if (routeStop == null)
            {
                return NotFound();
            }

            // Creating drop downs for Bus Stop Information when an existing routeStop is edited 

            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "RouteName", routeStop.BusRouteCode);
            var BusStopNumber = _context.BusStop.OrderBy(a => a.Location).Select(x => new { Text = x.Location + "  " + x.GoingDowntown, Value = x.BusStopNumber }).ToList();

            ViewBag.BusStopNumber = new SelectList(BusStopNumber, "Value", "Text");

            return View(routeStop);
        }

        // POST: BPRouteStop/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        // This Edit view gets called to save any edit changes made to a record of a stop of a particular bus Route
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RouteStopId,BusRouteCode,BusStopNumber,OffsetMinutes")] RouteStop routeStop)
        {
            if (id != routeStop.RouteStopId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(routeStop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RouteStopExists(routeStop.RouteStopId))
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
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode", routeStop.BusRouteCode);
            ViewData["BusStopNumber"] = new SelectList(_context.BusStop, "BusStopNumber", "BusStopNumber", routeStop.BusStopNumber);
            return View(routeStop);
        }

        // GET: BPRouteStop/Delete/5
        // The Delete view gets called when a stop of a bus Route is to be removed from that route
        // Parameter: busRouteCode as an int, which is the busRouteCode of the stop to be deleted
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeStop = await _context.RouteStop.SingleOrDefaultAsync(m => m.RouteStopId == id);
            if (routeStop == null)
            {
                return NotFound();
            }

            return View(routeStop);
        }

        // POST: BPRouteStop/Delete/5
        // This Delete view gets called when the user has confirmed the stop of the route to be deleted, and is then removed from the database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var routeStop = await _context.RouteStop.SingleOrDefaultAsync(m => m.RouteStopId == id);
                _context.RouteStop.Remove(routeStop);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["message"] = ex.InnerException.Message;
            }
            return RedirectToAction("Index");
        }

        private bool RouteStopExists(int id)
        {
            return _context.RouteStop.Any(e => e.RouteStopId == id);
        }
    }
}
