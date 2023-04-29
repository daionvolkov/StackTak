using StackTak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace StackTak.Controllers
{
    public class CreateController : Controller
    {
        private readonly InventoryDbContext db = new InventoryDbContext();

        public CreateController()
        {
            db = new InventoryDbContext();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(SwitchCreateViewModel switchCreateViewModel)
        {
            long ipAddrLong = ConvertIpToInt32(switchCreateViewModel.DeviceIPAddress);
            long ipGetwayLong = ConvertIpToInt32(switchCreateViewModel.IpGateway);

            var newAccessSwitch = new Access_Switch();
            var newAggregationSwitch = new Aggregation_Switch();

            if (ModelState.IsValid) {
                bool IsSwitchExists = false;
                bool IsGatwayExists = false;
                bool IsManufacturerExists = false;
                bool IsPostalAddressExists = false;

                if (switchCreateViewModel.TableType == SwitchTableType.Access) {
                    
                    IsSwitchExists = db.Access_Switches
                        .Any(s => s.Device_IP_Address == ipAddrLong);
                    IsGatwayExists = db.Aggregation_Switches
                        .Any(s => s.Device_IP_Address == ipGetwayLong);
                    IsManufacturerExists = db.Equipment_Manufacturers
                        .Any(s=>s.Hardware_Manufacturer == switchCreateViewModel.Manufacturer && s.Hardware_Model == switchCreateViewModel.DeviceModel);
                    IsPostalAddressExists = db.Postal_Addresses
                        .Any(s => s.City == switchCreateViewModel.City && s.Street == switchCreateViewModel.Street && s.Building == switchCreateViewModel.Building);

                }
                else if (switchCreateViewModel.TableType == SwitchTableType.Aggregation)
                {
                    IsSwitchExists = db.Aggregation_Switches.Any(s => s.Device_IP_Address == ipAddrLong);
                }
                if (IsSwitchExists) {
                    ModelState.AddModelError("", "Switch with this IP address already exists.");
                    return View(switchCreateViewModel);
                }

                if (switchCreateViewModel.TableType == SwitchTableType.Access && IsGatwayExists && IsManufacturerExists && IsPostalAddressExists) {

                    //newAccessSwitch.ID = db.Access_Switches.Max(i => i.ID) + 1;
                    newAccessSwitch.Device_IP_Address = ipAddrLong;
                    newAccessSwitch.Network_Mask = switchCreateViewModel.NetworkMask;
                    newAccessSwitch.IP_Gateway_ID = db.Aggregation_Switches
                        .Where(s => s.Device_IP_Address == ipGetwayLong)
                        .First().ID;
                    newAccessSwitch.Manufacturer_ID = db.Equipment_Manufacturers
                        .Where(s => s.Hardware_Manufacturer == switchCreateViewModel.Manufacturer && s.Hardware_Model == switchCreateViewModel.DeviceModel)
                        .First().Id;
                    newAccessSwitch.Description = switchCreateViewModel.Description;
                    newAccessSwitch.Postal_Address_ID = db.Postal_Addresses
                         .Where(s => s.City == switchCreateViewModel.City && s.Street == switchCreateViewModel.Street && s.Building == switchCreateViewModel.Building)
                         .First().Id;

                    db.Access_Switches.Add(newAccessSwitch);
                    db.SaveChanges();
                }
            }

            
            return RedirectToAction("Index", "Home");
        }



        public static long ConvertIpToInt32(string ipAddressString)
        {
            IPAddress ip = IPAddress.Parse(ipAddressString);
            byte[] bytes = ip.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}