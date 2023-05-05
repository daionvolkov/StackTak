using StackTak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace StackTak.Controllers
{
    public class DeleteController : Controller
    {
        private readonly InventoryDbContext db = new InventoryDbContext();
        public DeleteController()
        {
            db = new InventoryDbContext();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Delete(string IPAddr)
        {
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
        [ActionName("Delete")]
        public ActionResult Delete(EditViewModel viewModel) {

            if (viewModel.SwitchType == SwitchTypeEnum.Access)
            {
                Access_Switch accessSwitch = db.Access_Switches.Find(viewModel.Id);
                if (accessSwitch != null)
                {
                    db.Access_Switches.Remove(accessSwitch);
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