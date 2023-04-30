using StackTak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace StackTak.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        private readonly InventoryDbContext db = new InventoryDbContext();

        public SearchController()
        {
            db = new InventoryDbContext();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string city, string street, string building) {

           var res = db.Access_Switches.ToList()
             .Select(x => new { ipAddressString = LongToIpAddress(x.Device_IP_Address)});
             
            //TODO

            var accessSwitches = db.Access_Switches
             .Where(a => a.Postal_Address.City == city
                 || a.Postal_Address.Street == street
                 || a.Postal_Address.Building == building)
             .Select(a => new SearchViewModel
             {
                 IPAddress = res.ToString(),
                 //IPAddress = "192.168.1.1",
                 SubnetMask = a.Network_Mask,
                 Gateway = a.IP_Gateway_ID.ToString(),
                 Manufacturer = db.Equipment_Manufacturers
                     .Where(m => m.Id == a.Manufacturer_ID)
                     .Select(m => m.Hardware_Manufacturer + " " + m.Hardware_Model)
                     .FirstOrDefault(),
                 Description = a.Description
             })
             .ToList();

            ViewBag.City = city;
            ViewBag.Street = street;
            ViewBag.Building = building;

            return View(accessSwitches);
        }

        public static string LongToIpAddress(long ip)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ip >> 24 & 0xFF);
            sb.Append(".");
            sb.Append(ip >> 16 & 0xFF);
            sb.Append(".");
            sb.Append(ip >> 8 & 0xFF);
            sb.Append(".");
            sb.Append(ip & 0xFF);
            return sb.ToString();
        }
    }
}