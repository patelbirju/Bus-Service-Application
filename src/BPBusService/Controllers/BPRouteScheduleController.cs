using System;
using System.Collections.Generic;
using System.Data;
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
     *  BPRouteScheduleController is a controller which handles the schedules for the bus routes and stops in those particular routes
     *  It has full CRUD support and views
    */
    public class BPRouteScheduleController : Controller
    {
        private readonly BusServiceContext _context;

        public BPRouteScheduleController(BusServiceContext context)
        {
            _context = context;    
        }

        // GET: BPRouteSchedule
        // Index Action takes the busRouteCode and the routeName as parameters and start times for that particular route are displayed in the view
        public IActionResult Index(string busRouteCode = "", string routeName = "")
        {
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
            var routeSchedules = _context.RouteSchedule.Where((x => x.BusRouteCode == busRouteCode)).Include(x => x.BusRouteCodeNavigation);
            
            return View(routeSchedules.ToList());

        }

        // GET: BPRouteSchedule/Details/5
        // Shows the details of the start times of the route
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeSchedule = await _context.RouteSchedule.SingleOrDefaultAsync(m => m.RouteScheduleId == id);
            if (routeSchedule == null)
            {
                return NotFound();
            }

            return View(routeSchedule);
        }

        // GET: BPRouteSchedule/Create

        public IActionResult Create()
        {
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode");
            return View();
        }

        // POST: BPRouteSchedule/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RouteScheduleId,BusRouteCode,Comments,IsWeekDay,StartTime")] RouteSchedule routeSchedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(routeSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode", routeSchedule.BusRouteCode);
            return View(routeSchedule);
        }

        // GET: BPRouteSchedule/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeSchedule = await _context.RouteSchedule.SingleOrDefaultAsync(m => m.RouteScheduleId == id);
            if (routeSchedule == null)
            {
                return NotFound();
            }
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode", routeSchedule.BusRouteCode);
            return View(routeSchedule);
        }

        // POST: BPRouteSchedule/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RouteScheduleId,BusRouteCode,Comments,IsWeekDay,StartTime")] RouteSchedule routeSchedule)
        {
            if (id != routeSchedule.RouteScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(routeSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RouteScheduleExists(routeSchedule.RouteScheduleId))
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
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode", routeSchedule.BusRouteCode);
            return View(routeSchedule);
        }

        // GET: BPRouteSchedule/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeSchedule = await _context.RouteSchedule.SingleOrDefaultAsync(m => m.RouteScheduleId == id);
            if (routeSchedule == null)
            {
                return NotFound();
            }

            return View(routeSchedule);
        }

        // POST: BPRouteSchedule/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var routeSchedule = await _context.RouteSchedule.SingleOrDefaultAsync(m => m.RouteScheduleId == id);
            _context.RouteSchedule.Remove(routeSchedule);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Action Name: RouteStopSchedule
        // This action takes the routeStopId as the parameter
        // Based on the routeStopId, the schedule records are retrieved for the selected route
        // Offset minutes are added to the schedule's start times and thus the schedule for each stop is retrieved and displayed in the view 
        public IActionResult RouteStopSchedule(int routeStopId = 0)
        {
            if (routeStopId == 0)
            {
                TempData["message"] = "Please select a stop to see the schedule";
                return RedirectToAction("Index", "BPRouteStop");
            }

            //var routeStops = _context.RouteStop.Where(x => x.RouteStopId == routeStopId).Include(x => x.BusRouteCode).Include(x => x.BusStopNumber);

            RouteStop routeStop = _context.RouteStop.ToList().Find(x => x.RouteStopId == routeStopId);

            if (routeStop == null)
            {
                TempData["message"] = "Sorry, no Route Stop with that id";
                return RedirectToAction("Index", "BPRouteStop");
            }

            Response.Cookies.Append("busStopNumber", routeStop.BusStopNumber.ToString());
            Response.Cookies.Append("busRouteCode", routeStop.BusRouteCode);
           // Response.Cookies.Append("routeName", routeStop.BusRouteCodeNavigation.RouteName);
            var routeSchedules = _context.RouteSchedule.Where(x => x.BusRouteCode == (routeStop.BusRouteCode)).OrderBy(x => x.StartTime).ToList();

            if (routeSchedules.Count() == 0)
            {
                TempData["message"] = "There is no schedule for the selected route.";
                return RedirectToAction("Index", "GRBusStop");
            }
            foreach (var item in routeSchedules)
            {
                item.StartTime += new TimeSpan(0, (int)routeStop.OffsetMinutes, 0);
            }

            return View(routeSchedules);
        }


        private bool RouteScheduleExists(int id)
        {
            return _context.RouteSchedule.Any(e => e.RouteScheduleId == id);    
        }
    }
}
