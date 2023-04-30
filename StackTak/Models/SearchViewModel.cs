using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StackTak.Models
{
    public enum SwitchTypeEnum
    {
        Access,
        Aggregation
    }
    public class SearchViewModel
    {
        public string IPAddress { get; set; }
        public int SubnetMask { get; set; }
        public string Gateway { get; set; }
        public string Description { get; set; }
        public string Manufacturer { get; set; }
        public string DeviceModel { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public SwitchTypeEnum SwitchType { get; set;}
    }
}