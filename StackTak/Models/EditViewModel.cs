using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace StackTak.Models
{
    public enum SwitchType
    {
        Access,
        Aggregation
    }
    public class EditViewModel
    {
            public int Id { get; set; }
            [Required]
            [Display(Name = "Device IP Address")]
            public string IPAddress { get; set; }
            [Required]
            [Display(Name = "Subnet Mask")]
            [Range(0, 32, ErrorMessage = "Subnet Mask must be between 0 and 32")]
            public int SubnetMask { get; set; }
            [Required]
            [Display(Name = "IP Gateway")]
            public string Gateway { get; set; }
            [Display(Name = "Description")]
            public string Description { get; set; }
            [Required]
            [Display(Name = "Manufacturer")]
            public string Manufacturer { get; set; }
            [Required]
            [Display(Name = "Model")]
            public string DeviceModel { get; set; }
            [Required]
            [Display(Name = "City")]
            public string City { get; set; }
            [Required]
            [Display(Name = "Street")]
            public string Street { get; set; }
            [Required]
            [Display(Name = "Building")]
            public string Building { get; set; }
            [Required]
            [Display(Name = "Switch Type")]
            public SwitchTypeEnum SwitchType { get; set; }
        
    }
}