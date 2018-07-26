namespace TrekStories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddressIntoStreetAndCity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accommodation", "Street", c => c.String(maxLength: 100));
            AddColumn("dbo.Accommodation", "City", c => c.String(maxLength: 50));
            DropColumn("dbo.Accommodation", "Address");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accommodation", "Address", c => c.String(maxLength: 150));
            DropColumn("dbo.Accommodation", "City");
            DropColumn("dbo.Accommodation", "Street");
        }
    }
}
