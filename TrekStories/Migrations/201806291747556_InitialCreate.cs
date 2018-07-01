namespace TrekStories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accommodation",
                c => new
                    {
                        AccommodationId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 40),
                        Address = c.String(maxLength: 150),
                        PhoneNumber = c.String(),
                        CheckIn = c.DateTime(nullable: false),
                        CheckOut = c.DateTime(nullable: false),
                        ConfirmationFileUrl = c.String(),
                        Price = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.AccommodationId);
            
            CreateTable(
                "dbo.Step",
                c => new
                    {
                        StepId = c.Int(nullable: false, identity: true),
                        SequenceNo = c.Int(nullable: false),
                        From = c.String(maxLength: 20),
                        To = c.String(maxLength: 20),
                        WalkingTime = c.Double(nullable: false),
                        WalkingDistance = c.Double(nullable: false),
                        Ascent = c.Int(nullable: false),
                        Description = c.String(maxLength: 100),
                        Notes = c.String(maxLength: 500),
                        TripId = c.Int(nullable: false),
                        AccommodationId = c.Int(),
                    })
                .PrimaryKey(t => t.StepId)
                .ForeignKey("dbo.Accommodation", t => t.AccommodationId)
                .ForeignKey("dbo.Trip", t => t.TripId, cascadeDelete: true)
                .Index(t => t.TripId)
                .Index(t => t.AccommodationId);
            
            CreateTable(
                "dbo.Activity",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 40),
                        StartTime = c.DateTime(nullable: false),
                        Price = c.Double(nullable: false),
                        Notes = c.String(maxLength: 200),
                        StepId = c.Int(nullable: false),
                        Address = c.String(maxLength: 150),
                        LeisureCategory = c.Int(),
                        TransportType = c.Int(),
                        Company = c.String(),
                        Destination = c.String(),
                        Duration = c.Double(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Step", t => t.StepId, cascadeDelete: true)
                .Index(t => t.StepId);
            
            CreateTable(
                "dbo.Review",
                c => new
                    {
                        ReviewId = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                        PrivateNotes = c.String(maxLength: 2000),
                        PublicNotes = c.String(maxLength: 2000),
                        StepId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReviewId)
                .ForeignKey("dbo.Step", t => t.ReviewId)
                .Index(t => t.ReviewId);
            
            CreateTable(
                "dbo.Trip",
                c => new
                    {
                        TripId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 50),
                        Country = c.String(nullable: false),
                        TripCategory = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        Duration = c.Int(nullable: false),
                        Notes = c.String(maxLength: 500),
                        TotalCost = c.Double(nullable: false),
                        TotalWalkingDistance = c.Double(nullable: false),
                        TripOwner = c.String(),
                    })
                .PrimaryKey(t => t.TripId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Step", "TripId", "dbo.Trip");
            DropForeignKey("dbo.Review", "ReviewId", "dbo.Step");
            DropForeignKey("dbo.Activity", "StepId", "dbo.Step");
            DropForeignKey("dbo.Step", "AccommodationId", "dbo.Accommodation");
            DropIndex("dbo.Review", new[] { "ReviewId" });
            DropIndex("dbo.Activity", new[] { "StepId" });
            DropIndex("dbo.Step", new[] { "AccommodationId" });
            DropIndex("dbo.Step", new[] { "TripId" });
            DropTable("dbo.Trip");
            DropTable("dbo.Review");
            DropTable("dbo.Activity");
            DropTable("dbo.Step");
            DropTable("dbo.Accommodation");
        }
    }
}
