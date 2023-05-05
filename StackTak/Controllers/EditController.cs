using StackTak.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace StackTak.Controllers
{
    public class EditController : Controller
    {
        private readonly InventoryDbContext db = new InventoryDbContext();
        public EditController()
        {
            db = new InventoryDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Edit(string IPAddr) {
            bool isSwitchAccess = false;
            bool isSwitchAggregation = false;

            var viewModel = new EditViewModel();
            try
            {
                long ipAddressLong = IpAddressToLong(IPAddr);
                isSwitchAccess = db.Access_Switches
                     .Any(s => s.Device_IP_Address == ipAddressLong);

                isSwitchAggregation = db.Aggregation_Switches
                     .Any(s => s.Device_IP_Address == ipAddressLong);

                if (isSwitchAccess)
                {
                    var accessSwitch = db.Access_Switches.FirstOrDefault(s => s.Device_IP_Address == ipAddressLong);
                    var aggregationSwitch = db.Aggregation_Switches.FirstOrDefault(s => s.ID.Equals(accessSwitch.IP_Gateway_ID));
                    var manufacturer = db.Equipment_Manufacturers.FirstOrDefault(s => s.Id.Equals(accessSwitch.Manufacturer_ID));
                    var postAddress = db.Postal_Addresses.FirstOrDefault(s => s.Id.Equals(accessSwitch.Postal_Address_ID));

                    viewModel.Id = accessSwitch.ID;
                    viewModel.IPAddress = IPAddr;
                    viewModel.SubnetMask = accessSwitch.Network_Mask;
                    viewModel.Gateway = LongToIpAddress(aggregationSwitch.Device_IP_Address);
                    viewModel.Description = accessSwitch.Description;
                    viewModel.Manufacturer = manufacturer.Hardware_Manufacturer;
                    viewModel.DeviceModel = manufacturer.Hardware_Model;
                    viewModel.City = postAddress.City;
                    viewModel.Street = postAddress.Street;
                    viewModel.Building = postAddress.Building;
                    viewModel.SwitchType = SwitchTypeEnum.Access;
                }
                else if (isSwitchAggregation)
                {
                    var aggregationSwitch = db.Aggregation_Switches.FirstOrDefault(s => s.Device_IP_Address == ipAddressLong);
                    var manufacturer = db.Equipment_Manufacturers.FirstOrDefault(s => s.Id.Equals(aggregationSwitch.Manufacturer_ID));
                    var postAddress = db.Postal_Addresses.FirstOrDefault(s => s.Id.Equals(aggregationSwitch.Postal_Address_ID));
                    viewModel.Id = aggregationSwitch.ID;    
                    viewModel.IPAddress = IPAddr;
                    viewModel.SubnetMask = aggregationSwitch.Network_Mask;
                    viewModel.Gateway = "0";
                    viewModel.Description = aggregationSwitch.Description;
                    viewModel.Manufacturer = manufacturer.Hardware_Manufacturer;
                    viewModel.DeviceModel = manufacturer.Hardware_Model;
                    viewModel.City = postAddress.City;
                    viewModel.Street = postAddress.Street;
                    viewModel.Building = postAddress.Building;
                    viewModel.SwitchType = SwitchTypeEnum.Aggregation;
                }
            }
            catch
            {
                viewModel.IPAddress = "Not found";
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditViewModel viewModel) 
        {
            long ipGetwayLong = ConvertIpToInt32(viewModel.Gateway);

            var newManufacturer = new Equipment_Manufacturer();
            var newPostalAddress = new Postal_Address();
            bool isManufacturerExists = db.Equipment_Manufacturers
                .Any(s => s.Hardware_Manufacturer == viewModel.Manufacturer && s.Hardware_Model == viewModel.DeviceModel);
            bool isPostalAddressExists = db.Postal_Addresses
                .Any(s => s.City == viewModel.City && s.Street == viewModel.Street && s.Building == viewModel.Building);

            if (!isManufacturerExists && !isPostalAddressExists)
            {

                newManufacturer.Hardware_Manufacturer = viewModel.Manufacturer;
                newManufacturer.Hardware_Model = viewModel.DeviceModel;

                newPostalAddress.City = viewModel.City;
                newPostalAddress.Street = viewModel.Street;
                newPostalAddress.Building = viewModel.Building;

                db.Postal_Addresses.Add(newPostalAddress);
                db.Equipment_Manufacturers.Add(newManufacturer);
                db.SaveChanges();
                if (viewModel.SwitchType == SwitchTypeEnum.Access)
                {
                    var newAccessSwitch = new Access_Switch();
                    newAccessSwitch.ID = viewModel.Id;
                    newAccessSwitch.Device_IP_Address = ConvertIpToInt32(viewModel.IPAddress);
                    newAccessSwitch.Network_Mask = viewModel.SubnetMask;
                    newAccessSwitch.Description = viewModel.Description;
                    newAccessSwitch.IP_Gateway_ID = db.Aggregation_Switches
                            .Where(s => s.Device_IP_Address == ipGetwayLong)
                            .First().ID;
                    newAccessSwitch.Manufacturer_ID = db.Equipment_Manufacturers.Max(i => i.Id);
                    newAccessSwitch.Postal_Address_ID = db.Postal_Addresses.Max(i => i.Id);
                    db.Entry(newAccessSwitch).State = EntityState.Modified;
                    db.SaveChanges();                  
                }
                else if (viewModel.SwitchType == SwitchTypeEnum.Aggregation)
                {
                    var newAggregationSwitch = new Aggregation_Switch();
                    newAggregationSwitch.ID = viewModel.Id;
                    newAggregationSwitch.Device_IP_Address = ConvertIpToInt32(viewModel.IPAddress);
                    newAggregationSwitch.Network_Mask = viewModel.SubnetMask;
                    newAggregationSwitch.Description = viewModel.Description;
                    newAggregationSwitch.IP_Gateway = 0;
                    newAggregationSwitch.Manufacturer_ID = db.Equipment_Manufacturers.Max(i => i.Id);
                    newAggregationSwitch.Postal_Address_ID = db.Postal_Addresses.Max(i => i.Id);
                    db.Entry(newAggregationSwitch).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else if (isManufacturerExists && isPostalAddressExists) 
            {
                if (viewModel.SwitchType == SwitchTypeEnum.Access) 
                {
                    var newAccessSwitch = new Access_Switch();
                    newAccessSwitch.ID = viewModel.Id;
                    newAccessSwitch.Device_IP_Address = ConvertIpToInt32(viewModel.IPAddress);
                    newAccessSwitch.Network_Mask = viewModel.SubnetMask;
                    newAccessSwitch.Description = viewModel.Description;
                    newAccessSwitch.IP_Gateway_ID = db.Aggregation_Switches
                            .Where(s => s.Device_IP_Address == ipGetwayLong)
                            .First().ID;
                    newAccessSwitch.Manufacturer_ID = db.Equipment_Manufacturers
                            .Where(s => s.Hardware_Manufacturer == viewModel.Manufacturer && s.Hardware_Model == viewModel.DeviceModel)
                            .First().Id;
                    newAccessSwitch.Postal_Address_ID = db.Postal_Addresses
                             .Where(s => s.City == viewModel.City && s.Street == viewModel.Street && s.Building == viewModel.Building)
                             .First().Id;
                    db.Entry(newAccessSwitch).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (viewModel.SwitchType == SwitchTypeEnum.Aggregation)
                {
                    var newAggregationSwitch = new Aggregation_Switch();
                    newAggregationSwitch.ID = viewModel.Id;
                    newAggregationSwitch.Device_IP_Address = ConvertIpToInt32(viewModel.IPAddress);
                    newAggregationSwitch.Network_Mask = viewModel.SubnetMask;
                    newAggregationSwitch.Description = viewModel.Description;
                    newAggregationSwitch.IP_Gateway = 0;
                    newAggregationSwitch.Manufacturer_ID = db.Equipment_Manufacturers
                           .Where(s => s.Hardware_Manufacturer == viewModel.Manufacturer && s.Hardware_Model == viewModel.DeviceModel)
                           .First().Id;
                    newAggregationSwitch.Postal_Address_ID = db.Postal_Addresses
                             .Where(s => s.City == viewModel.City && s.Street == viewModel.Street && s.Building == viewModel.Building)
                             .First().Id;
                    db.Entry(newAggregationSwitch).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else if (!isManufacturerExists && isPostalAddressExists)
            {
                newManufacturer.Hardware_Manufacturer = viewModel.Manufacturer;
                newManufacturer.Hardware_Model = viewModel.DeviceModel;
                db.Equipment_Manufacturers.Add(newManufacturer);
                db.SaveChanges();

                if (viewModel.SwitchType == SwitchTypeEnum.Access)
                {
                    var newAccessSwitch = new Access_Switch();
                    newAccessSwitch.ID = viewModel.Id;
                    newAccessSwitch.Device_IP_Address = ConvertIpToInt32(viewModel.IPAddress);
                    newAccessSwitch.Network_Mask = viewModel.SubnetMask;
                    newAccessSwitch.Description = viewModel.Description;
                    newAccessSwitch.IP_Gateway_ID = db.Aggregation_Switches
                            .Where(s => s.Device_IP_Address == ipGetwayLong)
                            .First().ID;
                    newAccessSwitch.Manufacturer_ID = db.Equipment_Manufacturers.Max(i => i.Id);
                    newAccessSwitch.Postal_Address_ID = db.Postal_Addresses
                             .Where(s => s.City == viewModel.City && s.Street == viewModel.Street && s.Building == viewModel.Building)
                             .First().Id;
                    db.Entry(newAccessSwitch).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (viewModel.SwitchType == SwitchTypeEnum.Aggregation)
                {
                    var newAggregationSwitch = new Aggregation_Switch();
                    newAggregationSwitch.ID = viewModel.Id;
                    newAggregationSwitch.Device_IP_Address = ConvertIpToInt32(viewModel.IPAddress);
                    newAggregationSwitch.Network_Mask = viewModel.SubnetMask;
                    newAggregationSwitch.Description = viewModel.Description;
                    newAggregationSwitch.IP_Gateway = 0;
                    newAggregationSwitch.Manufacturer_ID = db.Equipment_Manufacturers.Max(i => i.Id);
                    newAggregationSwitch.Postal_Address_ID = db.Postal_Addresses
                             .Where(s => s.City == viewModel.City && s.Street == viewModel.Street && s.Building == viewModel.Building)
                             .First().Id;
                    db.Entry(newAggregationSwitch).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else if (isManufacturerExists && !isPostalAddressExists)
            {
                newPostalAddress.City = viewModel.City;
                newPostalAddress.Street = viewModel.Street;
                newPostalAddress.Building = viewModel.Building;

                db.Postal_Addresses.Add(newPostalAddress);
                db.SaveChanges();

                if (viewModel.SwitchType == SwitchTypeEnum.Access)
                {
                    var newAccessSwitch = new Access_Switch();
                    newAccessSwitch.ID = viewModel.Id;
                    newAccessSwitch.Device_IP_Address = ConvertIpToInt32(viewModel.IPAddress);
                    newAccessSwitch.Network_Mask = viewModel.SubnetMask;
                    newAccessSwitch.Description = viewModel.Description;
                    newAccessSwitch.IP_Gateway_ID = db.Aggregation_Switches
                            .Where(s => s.Device_IP_Address == ipGetwayLong)
                            .First().ID;
                    newAccessSwitch.Manufacturer_ID = db.Equipment_Manufacturers
                           .Where(s => s.Hardware_Manufacturer == viewModel.Manufacturer && s.Hardware_Model == viewModel.DeviceModel)
                           .First().Id;
                    newAccessSwitch.Postal_Address_ID = db.Postal_Addresses.Max(i => i.Id);
                    db.Entry(newAccessSwitch).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (viewModel.SwitchType == SwitchTypeEnum.Aggregation)
                {
                    var newAggregationSwitch = new Aggregation_Switch();
                    newAggregationSwitch.ID = viewModel.Id;
                    newAggregationSwitch.Device_IP_Address = ConvertIpToInt32(viewModel.IPAddress);
                    newAggregationSwitch.Network_Mask = viewModel.SubnetMask;
                    newAggregationSwitch.Description = viewModel.Description;
                    newAggregationSwitch.IP_Gateway = 0;
                    newAggregationSwitch.Manufacturer_ID = db.Equipment_Manufacturers
                           .Where(s => s.Hardware_Manufacturer == viewModel.Manufacturer && s.Hardware_Model == viewModel.DeviceModel)
                           .First().Id;
                    newAggregationSwitch.Postal_Address_ID = db.Postal_Addresses.Max(i => i.Id);
                    db.Entry(newAggregationSwitch).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public static long IpAddressToLong(string ipAddress)
        {
            IPAddress ip = IPAddress.Parse(ipAddress);
            byte[] bytes = ip.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToUInt32(bytes, 0);
        }
        public static string LongToIpAddress(long ip)
        {
            IPAddress ipAddress = IPAddress.Parse(ip.ToString());
            return ipAddress.ToString();
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