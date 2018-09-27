namespace TrekStories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReviewImagesAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Image",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        ReviewId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Review", t => t.ReviewId, cascadeDelete: true)
                .Index(t => t.ReviewId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Image", "ReviewId", "dbo.Review");
            DropIndex("dbo.Image", new[] { "ReviewId" });
            DropTable("dbo.Image");
        }
    }
}
