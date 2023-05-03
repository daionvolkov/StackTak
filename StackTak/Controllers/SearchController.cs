using StackTak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace StackTak.Controllers
{
    public class SearchController : Controller
    {
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

            var searchResults = new List<SearchViewModel>();

            var query = from accessSwitch in db.Access_Switches
                        join aggregationSwitch in db.Aggregation_Switches on accessSwitch.IP_Gateway_ID equals aggregationSwitch.ID
                        join postalAddress in db.Postal_Addresses on accessSwitch.Postal_Address_ID equals postalAddress.Id
                        join equipment in db.Equipment_Manufacturers on accessSwitch.Manufacturer_ID equals equipment.Id
                        where (city == null || postalAddress.City.Contains(city))
                            && (street == null || postalAddress.Street.Contains(street))
                            && (building == null || postalAddress.Building.Contains(building))
                        select new
                        {
                            IpAddress = accessSwitch.Device_IP_Address,
                            SubnetMask = accessSwitch.Network_Mask,
                            Gateway = aggregationSwitch.Device_IP_Address,
                            Description = accessSwitch.Description,
                            Manufacturer = equipment.Hardware_Manufacturer,
                            Model = equipment.Hardware_Model,
                            City = postalAddress.City,
                            Street = postalAddress.Street,
                            Building = postalAddress.Building
                        };

            foreach (var result in query)
            {
                searchResults.Add(new SearchViewModel
                {
                    IPAddress = ConvertIpAddress(result.IpAddress),
                    SubnetMask = result.SubnetMask,
                    Gateway = ConvertIpAddress(result.Gateway),
                    Description = result.Description,
                    Manufacturer = result.Manufacturer,
                    DeviceModel = result.Model,
                    City = result.City,
                    Street = result.Street,
                    Building = result.Building
                });
            }
            return View(searchResults);
        }

        private static string ConvertIpAddress(long ipAddress)
        {
            var octets = new byte[4];

            octets[0] = (byte)(ipAddress >> 24);
            octets[1] = (byte)(ipAddress >> 16);
            octets[2] = (byte)(ipAddress >> 8);
            octets[3] = (byte)(ipAddress);

            return $"{octets[0]}.{octets[1]}.{octets[2]}.{octets[3]}";
        }

    }
}