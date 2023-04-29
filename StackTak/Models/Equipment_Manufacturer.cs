using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StackTak.Models
{
    [Table("Equipment_Manufacturer")]
    public class Equipment_Manufacturer
    {
        [Key]
        public int Id { get; set; }
        public string Hardware_Manufacturer { get; set; }
        public string Hardware_Model { get; set; }
    }
}