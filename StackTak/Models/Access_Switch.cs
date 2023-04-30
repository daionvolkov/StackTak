using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Web;

namespace StackTak.Models
{
    [Table("Access_Switch")]
    public class Access_Switch
    {
        [Key]
        public int ID { get; set; }
        public long Device_IP_Address { get; set; }
        public int Network_Mask { get; set; }
        public int IP_Gateway_ID { get; set; } = 0;
        public int Manufacturer_ID { get; set; }
        public string Description { get; set; }
        public int Postal_Address_ID { get; set; }

        public Postal_Address Postal_Address { get; set; }
    }
}