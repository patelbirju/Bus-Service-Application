using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BPBusService.Models;

namespace BPBusService.Controllers
{
    public class HomeController : Controller
    {
        private readonly BusServiceContext _context;
        public HomeController(BusServiceContext context)
        {
            _context = context;
        }
        public IActionResult Sample(string busRouteCode)
        {
            var busRoutes = _context.BusRoute.Where(x => x.BusRouteCode == busRouteCode);
            //var busRoutes = from bus in _context.BusRoute
              //            select bus;


            return View(busRoutes);

        }


        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
