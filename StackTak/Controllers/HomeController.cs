using StackTak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;

namespace StackTak.Controllers
{
    public class HomeController : Controller
    {
        private readonly InventoryDbContext db = new InventoryDbContext();

        public HomeController()
        {
            db = new InventoryDbContext();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string ipAddress)
        {
            bool isSwitchAccess = false;
            bool isSwitchAggregation = false;

            var viewModel = new SearchViewModel();
            try
            {
                long ipAddressLong = IpAddressToLong(ipAddress);
                isSwitchAccess =  db.Access_Switches
                        .Any(s => s.Device_IP_Address == ipAddressLong);

                isSwitchAggregation = db.Aggregation_Switches
                     .Any(s => s.Device_IP_Address == ipAddressLong);

                if (isSwitchAccess)
                {
                    var accessSwitch = db.Access_Switches.FirstOrDefault(s => s.Device_IP_Address == ipAddressLong);
                    var aggregationSwitch = db.Aggregation_Switches.FirstOrDefault(s => s.ID.Equals(accessSwitch.IP_Gateway_ID));
                    var manufacturer = db.Equipment_Manufacturers.FirstOrDefault(s => s.Id.Equals(accessSwitch.Manufacturer_ID));
                    var postAddress = db.Postal_Addresses.FirstOrDefault(s => s.Id.Equals(accessSwitch.Postal_Address_ID));
                
                        viewModel.IPAddress = ipAddress;
                        viewModel.SubnetMask = accessSwitch.Network_Mask;
                        viewModel.Gateway = LongToIpAddress(aggregationSwitch.Device_IP_Address);
                        //viewModel.Description = accessSwitch.Description;
                        //viewModel.Manufacturer = manufacturer.Hardware_Manufacturer;
                        //viewModel.DeviceModel = manufacturer.Hardware_Model;
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
                   
                        viewModel.IPAddress = ipAddress;
                        viewModel.SubnetMask = aggregationSwitch.Network_Mask;
                        viewModel.Gateway = "0";
                        //viewModel.Description = aggregationSwitch.Description;
                        //viewModel.Manufacturer = manufacturer.Hardware_Manufacturer;
                        //viewModel.DeviceModel = manufacturer.Hardware_Model;
                        viewModel.City = postAddress.City;
                        viewModel.Street = postAddress.Street;
                        viewModel.Building = postAddress.Building;
                        viewModel.SwitchType = SwitchTypeEnum.Aggregation;
                }

            }
            catch {
                viewModel.IPAddress = "Not found";
            }
            return View(viewModel);
            
        }

        public ActionResult Details(string IPAddr)
        {
            bool isSwitchAccess = false;
            bool isSwitchAggregation = false;

            var viewModel = new SearchViewModel();
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
        


       /* public static string DecimalToBinary(int decimalNumber)
        {
            int[] binaryNumber = new int;
            int index = 0;
            while (decimalNumber > 0)
            {
                binaryNumber[index++] = decimalNumber % 2;
                decimalNumber = decimalNumber / 2;
            }
            Array.Reverse(binaryNumber, 0, index);
            return string.Join(".", binaryNumber.Select(x => x.ToString()).ToArray());
        }*/

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);    
        }
    }
}