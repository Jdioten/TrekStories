namespace TrekStories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class increaseDescriptionLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Step", "Description", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Step", "Description", c => c.String(maxLength: 100));
        }
    }
}
