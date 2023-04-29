using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace StackTak.Models
{
    public class InventoryDbContext : DbContext
    {
     
        public DbSet<Access_Switch> Access_Switches { get; set; }
        public DbSet<Postal_Address> Postal_Addresses { get; set; }

        public DbSet<Aggregation_Switch> Aggregation_Switches { get; set; }
        public DbSet<Equipment_Manufacturer> Equipment_Manufacturers { get; set; }

    }
}