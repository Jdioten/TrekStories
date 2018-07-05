namespace TrekStories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequiredAttributeForNonNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Step", "From", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Step", "To", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Activity", "Name", c => c.String(nullable: false, maxLength: 40));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Activity", "Name", c => c.String(maxLength: 40));
            AlterColumn("dbo.Step", "To", c => c.String(maxLength: 20));
            AlterColumn("dbo.Step", "From", c => c.String(maxLength: 20));
        }
    }
}
