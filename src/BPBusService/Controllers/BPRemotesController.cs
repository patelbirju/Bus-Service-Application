using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BPBusService.Models;
using Microsoft.EntityFrameworkCore;
/// <summary>
/// BPRemotes controller which has the action method for the province code
/// The action method and the controller will be used for a Remote annotation validation in the BPDriver Metadata class
/// </summary>
namespace BPBusService.Controllers
{
    public class BPRemotesController : Controller
    {
        private readonly BusServiceContext _context;

        public BPRemotesController(BusServiceContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public JsonResult checkProvinceCode(string provinceCode)
        {
            if(provinceCode.Length != 2)
            {
                return Json("Province code must be exactly 2 letters"); 
            }

            // Check if province code exists in the province table
            try
            {
                var provinceCodeEntered = _context.Province.Where(p => p.ProvinceCode == provinceCode);
                if (provinceCodeEntered.Count() == 0)
                {
                    return Json("Province Code not on file");
                }
            }
            catch(Exception ex)
            {
                return Json("Error validating province code" + ex.GetBaseException());
            }
            

            return Json(true);
        }
    
    }
}