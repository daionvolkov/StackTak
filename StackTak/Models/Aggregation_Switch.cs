using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Web;

namespace StackTak.Models
{
    [Table("Aggregation_Switch")]
    public class Aggregation_Switch
    {
        [Key]
        public int ID { get; set; }
        public long Device_IP_Address { get; set; }
        public int Network_Mask { get; set; }
        public long IP_Gateway { get; set; }
        public int Manufacturer_ID { get; set; }
        public string Description { get; set; }
        public int Postal_Address_ID { get; set; }

      // public virtual IEnumerable<Access_Switch> AccessSwitch { get; set; }
    }
}