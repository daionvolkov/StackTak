using StackTak.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace StackTak.Controllers
{
    [Authorize]
    public class CreateController : Controller
    {
        private readonly InventoryDbContext db = new InventoryDbContext();
        public CreateController()
        {
            db = new InventoryDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(SwitchCreateViewModel switchCreateViewModel)
        {
      
            var newAccessSwitch = new Access_Switch();
            var newAggregationSwitch = new Aggregation_Switch();
            var newManufacturer = new Equipment_Manufacturer();
            var newPostalAddress = new Postal_Address();

            if (ModelState.IsValid) {
                bool isSwitchExists = false;
                bool isGatwayExists = false;
                bool isManufacturerExists = false;
                bool isPostalAddressExists = false;

                if (switchCreateViewModel.TableType == SwitchTableType.Access) {
                    
                    long ipAddrLong = ConvertIpToInt32(switchCreateViewModel.DeviceIPAddress);
                    long ipGetwayLong = ConvertIpToInt32(switchCreateViewModel.IpGateway);

                    isSwitchExists = db.Access_Switches
                        .Any(s => s.Device_IP_Address == ipAddrLong);
                    isGatwayExists = db.Aggregation_Switches
                        .Any(s => s.Device_IP_Address == ipGetwayLong);
                    isManufacturerExists = db.Equipment_Manufacturers
                        .Any(s=>s.Hardware_Manufacturer == switchCreateViewModel.Manufacturer && s.Hardware_Model == switchCreateViewModel.DeviceModel);
                    isPostalAddressExists = db.Postal_Addresses
                        .Any(s => s.City == switchCreateViewModel.City && s.Street == switchCreateViewModel.Street && s.Building == switchCreateViewModel.Building);
                    
                    if (isSwitchExists)
                    {
                        ModelState.AddModelError("", "Switch with this IP address already exists.");
                        return View(switchCreateViewModel);
                    }
                    if (isGatwayExists && isManufacturerExists && isPostalAddressExists)
                    {

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
                    // case when the equipment is not in the database, postal address is exists
                    else if (isGatwayExists && !isManufacturerExists && isPostalAddressExists)
                    {

                        newManufacturer.Hardware_Manufacturer = switchCreateViewModel.Manufacturer;
                        newManufacturer.Hardware_Model = switchCreateViewModel.DeviceModel;

                        db.Equipment_Manufacturers.Add(newManufacturer);
                        db.SaveChanges();

                        newAccessSwitch.Device_IP_Address = ipAddrLong;
                        newAccessSwitch.Network_Mask = switchCreateViewModel.NetworkMask;
                        newAccessSwitch.IP_Gateway_ID = db.Aggregation_Switches
                            .Where(s => s.Device_IP_Address == ipGetwayLong)
                            .First().ID;
                        newAccessSwitch.Manufacturer_ID = db.Equipment_Manufacturers.Max(i => i.Id);
                        newAccessSwitch.Description = switchCreateViewModel.Description;
                        newAccessSwitch.Postal_Address_ID = db.Postal_Addresses
                             .Where(s => s.City == switchCreateViewModel.City && s.Street == switchCreateViewModel.Street && s.Building == switchCreateViewModel.Building)
                             .First().Id;

                        db.Access_Switches.Add(newAccessSwitch);
                        db.SaveChanges();
                    }
                    // case when the equipment is not in the database, postal address is not in database
                    else if (isGatwayExists && !isManufacturerExists && !isPostalAddressExists)
                    {
                        newPostalAddress.City = switchCreateViewModel.City;
                        newPostalAddress.Street = switchCreateViewModel.Street;
                        newPostalAddress.Building = switchCreateViewModel.Building;

                        db.Postal_Addresses.Add(newPostalAddress);
                        db.SaveChanges();

                        newManufacturer.Hardware_Manufacturer = switchCreateViewModel.Manufacturer;
                        newManufacturer.Hardware_Model = switchCreateViewModel.DeviceModel;

                        db.Equipment_Manufacturers.Add(newManufacturer);
                        db.SaveChanges();

                        newAccessSwitch.Device_IP_Address = ipAddrLong;
                        newAccessSwitch.Network_Mask = switchCreateViewModel.NetworkMask;
                        newAccessSwitch.IP_Gateway_ID = db.Aggregation_Switches
                            .Where(s => s.Device_IP_Address == ipGetwayLong)
                            .First().ID;
                        newAccessSwitch.Manufacturer_ID = db.Equipment_Manufacturers.Max(i => i.Id);
                        newAccessSwitch.Description = switchCreateViewModel.Description;
                        newAccessSwitch.Postal_Address_ID = db.Postal_Addresses.Max(i => i.Id);

                        db.Access_Switches.Add(newAccessSwitch);
                        db.SaveChanges();
                    }
                    // case when the equipment is exists in the database, postal address is not in database
                    else if (isGatwayExists && isManufacturerExists && !isPostalAddressExists)
                    {
                        newPostalAddress.City = switchCreateViewModel.City;
                        newPostalAddress.Street = switchCreateViewModel.Street;
                        newPostalAddress.Building = switchCreateViewModel.Building;

                        db.Postal_Addresses.Add(newPostalAddress);
                        db.SaveChanges();

                        newAccessSwitch.Device_IP_Address = ipAddrLong;
                        newAccessSwitch.Network_Mask = switchCreateViewModel.NetworkMask;
                        newAccessSwitch.IP_Gateway_ID = db.Aggregation_Switches
                            .Where(s => s.Device_IP_Address == ipGetwayLong)
                            .First().ID;
                        newAccessSwitch.Manufacturer_ID = db.Equipment_Manufacturers
                            .Where(s => s.Hardware_Manufacturer == switchCreateViewModel.Manufacturer && s.Hardware_Model == switchCreateViewModel.DeviceModel)
                            .First().Id;
                        newAccessSwitch.Description = switchCreateViewModel.Description;
                        newAccessSwitch.Postal_Address_ID = db.Postal_Addresses.Max(i => i.Id);

                        db.Access_Switches.Add(newAccessSwitch);
                        db.SaveChanges();
                    }
                }     
               
                if (switchCreateViewModel.TableType == SwitchTableType.Aggregation) {

                    long ipAddrLong = ConvertIpToInt32(switchCreateViewModel.DeviceIPAddress);

                    isSwitchExists = db.Aggregation_Switches
                        .Any(s => s.Device_IP_Address == ipAddrLong);
                    isManufacturerExists = db.Equipment_Manufacturers
                        .Any(s => s.Hardware_Manufacturer == switchCreateViewModel.Manufacturer && s.Hardware_Model == switchCreateViewModel.DeviceModel);
                    isPostalAddressExists = db.Postal_Addresses
                        .Any(s => s.City == switchCreateViewModel.City && s.Street == switchCreateViewModel.Street && s.Building == switchCreateViewModel.Building);

                    if (isSwitchExists)
                    {
                        ModelState.AddModelError("", "Switch with this IP address already exists.");
                        return View(switchCreateViewModel);
                    }
                    if (isManufacturerExists && isPostalAddressExists)
                    {
                        newAggregationSwitch.Device_IP_Address = ipAddrLong;
                        newAggregationSwitch.Network_Mask = switchCreateViewModel.NetworkMask;
                        newAggregationSwitch.IP_Gateway = int.Parse(switchCreateViewModel.IpGateway); ;
                        newAggregationSwitch.Manufacturer_ID = db.Equipment_Manufacturers
                            .Where(s => s.Hardware_Manufacturer == switchCreateViewModel.Manufacturer && s.Hardware_Model == switchCreateViewModel.DeviceModel)
                            .First().Id;
                        newAggregationSwitch.Description = switchCreateViewModel.Description;
                        newAggregationSwitch.Postal_Address_ID = db.Postal_Addresses
                             .Where(s => s.City == switchCreateViewModel.City && s.Street == switchCreateViewModel.Street && s.Building == switchCreateViewModel.Building)
                             .First().Id;

                        db.Aggregation_Switches.Add(newAggregationSwitch);
                        db.SaveChanges();
                    }
                    else if (!isManufacturerExists && isPostalAddressExists)
                    {

                        newManufacturer.Hardware_Manufacturer = switchCreateViewModel.Manufacturer;
                        newManufacturer.Hardware_Model = switchCreateViewModel.DeviceModel;

                        db.Equipment_Manufacturers.Add(newManufacturer);
                        db.SaveChanges();

                        newAggregationSwitch.Device_IP_Address = ipAddrLong;
                        newAggregationSwitch.Network_Mask = switchCreateViewModel.NetworkMask;
                        newAggregationSwitch.IP_Gateway = int.Parse(switchCreateViewModel.IpGateway); ;
                        newAggregationSwitch.Manufacturer_ID = db.Equipment_Manufacturers.Max(i => i.Id);
                        newAggregationSwitch.Description = switchCreateViewModel.Description;
                        newAggregationSwitch.Postal_Address_ID = db.Postal_Addresses
                             .Where(s => s.City == switchCreateViewModel.City && s.Street == switchCreateViewModel.Street && s.Building == switchCreateViewModel.Building)
                             .First().Id;

                        db.Aggregation_Switches.Add(newAggregationSwitch);
                        db.SaveChanges();
                    }
                    else if (!isManufacturerExists && !isPostalAddressExists)
                    {
                        newPostalAddress.City = switchCreateViewModel.City;
                        newPostalAddress.Street = switchCreateViewModel.Street;
                        newPostalAddress.Building = switchCreateViewModel.Building;

                        db.Postal_Addresses.Add(newPostalAddress);
                        db.SaveChanges();

                        newManufacturer.Hardware_Manufacturer = switchCreateViewModel.Manufacturer;
                        newManufacturer.Hardware_Model = switchCreateViewModel.DeviceModel;

                        db.Equipment_Manufacturers.Add(newManufacturer);
                        db.SaveChanges();

                        newAggregationSwitch.Device_IP_Address = ipAddrLong;
                        newAggregationSwitch.Network_Mask = switchCreateViewModel.NetworkMask;
                        newAggregationSwitch.IP_Gateway = int.Parse(switchCreateViewModel.IpGateway); 
                        newAggregationSwitch.Manufacturer_ID = db.Equipment_Manufacturers.Max(i => i.Id);
                        newAggregationSwitch.Description = switchCreateViewModel.Description;
                        newAggregationSwitch.Postal_Address_ID = db.Postal_Addresses.Max(i => i.Id);

                        db.Aggregation_Switches.Add(newAggregationSwitch);
                        db.SaveChanges();
                    }
                    // case when the equipment is exists in the database, postal address is not in database
                    else if (isManufacturerExists && !isPostalAddressExists)
                    {
                        newPostalAddress.City = switchCreateViewModel.City;
                        newPostalAddress.Street = switchCreateViewModel.Street;
                        newPostalAddress.Building = switchCreateViewModel.Building;

                        db.Postal_Addresses.Add(newPostalAddress);
                        db.SaveChanges();

                        newAggregationSwitch.Device_IP_Address = ipAddrLong;
                        newAggregationSwitch.Network_Mask = switchCreateViewModel.NetworkMask;
                        newAggregationSwitch.IP_Gateway = int.Parse(switchCreateViewModel.IpGateway);
                        newAggregationSwitch.Manufacturer_ID = db.Equipment_Manufacturers
                            .Where(s => s.Hardware_Manufacturer == switchCreateViewModel.Manufacturer && s.Hardware_Model == switchCreateViewModel.DeviceModel)
                            .First().Id;
                        newAggregationSwitch.Description = switchCreateViewModel.Description;
                        newAggregationSwitch.Postal_Address_ID = db.Postal_Addresses.Max(i => i.Id);

                        db.Aggregation_Switches.Add(newAggregationSwitch);
                        db.SaveChanges();
                    }
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