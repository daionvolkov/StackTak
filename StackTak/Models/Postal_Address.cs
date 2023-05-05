using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StackTak.Models
{
    [Table("Postal_Address")]
    public class Postal_Address
    {
        [Key]
        public int Id { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }

       // public List<Access_Switch> Access_Switches { get; set; } = new List<Access_Switch>();

    }
}