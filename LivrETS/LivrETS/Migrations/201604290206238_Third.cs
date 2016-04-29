namespace LivrETS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Third : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Offers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        Price = c.Single(nullable: false),
                        Condition = c.String(nullable: false),
                        MarkedSoldOn = c.DateTime(nullable: false),
                        Article_Id = c.Guid(nullable: false),
                        Fair_Id = c.Guid(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Articles", t => t.Article_Id, cascadeDelete: true)
                .ForeignKey("dbo.Fairs", t => t.Fair_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.Article_Id)
                .Index(t => t.Fair_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 256),
                        FairState = c.Int(nullable: false),
                        ISBN = c.String(),
                        Model = c.Int(),
                        SubTitle = c.String(),
                        BarCode = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Course_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.Course_Id)
                .Index(t => t.Course_Id);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Acronym = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Offers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Offers", "Fair_Id", "dbo.Fairs");
            DropForeignKey("dbo.Offers", "Article_Id", "dbo.Articles");
            DropForeignKey("dbo.Articles", "Course_Id", "dbo.Courses");
            DropIndex("dbo.Articles", new[] { "Course_Id" });
            DropIndex("dbo.Offers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Offers", new[] { "Fair_Id" });
            DropIndex("dbo.Offers", new[] { "Article_Id" });
            DropTable("dbo.Courses");
            DropTable("dbo.Articles");
            DropTable("dbo.Offers");
        }
    }
}
