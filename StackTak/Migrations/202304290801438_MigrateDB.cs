namespace StackTak.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Access_Switch",
                c => new
                    {
                        Device_IP_Address = c.Long(nullable: false),
                        Network_Mask = c.Int(nullable: false),
                        IP_Gateway_ID = c.Int(nullable: false),
                        Manufacturer_ID = c.Int(nullable: false),
                        Description = c.String(),
                        Postal_Address_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Device_IP_Address);
            
            CreateTable(
                "dbo.Aggregation_Switch",
                c => new
                    {
                        Device_IP_Address = c.Long(nullable: false),
                        Network_Mask = c.Int(nullable: false),
                        IP_Gateway = c.Int(nullable: false),
                        Manufacturer_ID = c.Int(nullable: false),
                        Description = c.String(),
                        Postal_Address_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Device_IP_Address);
            
            CreateTable(
                "dbo.Equipment_Manufacturer",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Hardware_Manufacturer = c.String(),
                        Hardware_Model = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Postal_Address",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        City = c.String(),
                        Street = c.String(),
                        Building = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Postal_Address");
            DropTable("dbo.Equipment_Manufacturer");
            DropTable("dbo.Aggregation_Switch");
            DropTable("dbo.Access_Switch");
        }
    }
}
