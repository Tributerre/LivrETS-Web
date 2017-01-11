namespace LivrETS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 256),
                        DeletedAt = c.DateTime(nullable: false),
                        Price = c.Double(nullable: false),
                        FairState = c.Int(nullable: false),
                        CourseID = c.Guid(nullable: false),
                        GeneratedNumber = c.Long(nullable: false, identity: true),
                        ISBN = c.String(),
                        Model = c.Int(),
                        SubTitle = c.String(),
                        BarCode = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.CourseID, cascadeDelete: true)
                .Index(t => t.CourseID);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Acronym = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Fairs",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Trimester = c.String(nullable: false),
                        CommissionOnSale = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FairSteps",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        FairID = c.Guid(nullable: false),
                        Place = c.String(nullable: false),
                        Phase = c.String(nullable: false),
                        StartDateTime = c.DateTime(nullable: false),
                        EndDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Fairs", t => t.FairID, cascadeDelete: true)
                .Index(t => t.FairID);
            
            CreateTable(
                "dbo.Offers",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        Price = c.Single(nullable: false),
                        Title = c.String(nullable: false, maxLength: 256),
                        Condition = c.String(nullable: false),
                        MarkedSoldOn = c.DateTime(nullable: false),
                        ArticleID = c.Guid(nullable: false),
                        ManagedByFair = c.Boolean(nullable: false),
                        Fair_Id = c.Guid(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Articles", t => t.ArticleID, cascadeDelete: true)
                .ForeignKey("dbo.Fairs", t => t.Fair_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ArticleID)
                .Index(t => t.Fair_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.OfferImages",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        PathOnDisk = c.String(nullable: false),
                        ThumbnailPathOnDisk = c.String(nullable: false),
                        Offer_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Offers", t => t.Offer_Id)
                .Index(t => t.Offer_Id);
            
            CreateTable(
                "dbo.Sales",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        FairID = c.Guid(nullable: false),
                        SellerID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Fairs", t => t.FairID, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.SellerID)
                .Index(t => t.FairID)
                .Index(t => t.SellerID);
            
            CreateTable(
                "dbo.SaleItems",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        OfferID = c.Guid(nullable: false),
                        Sale_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Offers", t => t.OfferID, cascadeDelete: true)
                .ForeignKey("dbo.Sales", t => t.Sale_Id)
                .Index(t => t.OfferID)
                .Index(t => t.Sale_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false, maxLength: 256),
                        LastName = c.String(nullable: false, maxLength: 256),
                        SubscribedAt = c.DateTime(nullable: false),
                        BarCode = c.String(nullable: false),
                        GeneratedNumber = c.Long(nullable: false, identity: true),
                        LivrETSID = c.String(maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Sales", "SellerID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Offers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SaleItems", "Sale_Id", "dbo.Sales");
            DropForeignKey("dbo.SaleItems", "OfferID", "dbo.Offers");
            DropForeignKey("dbo.Sales", "FairID", "dbo.Fairs");
            DropForeignKey("dbo.Offers", "Fair_Id", "dbo.Fairs");
            DropForeignKey("dbo.OfferImages", "Offer_Id", "dbo.Offers");
            DropForeignKey("dbo.Offers", "ArticleID", "dbo.Articles");
            DropForeignKey("dbo.FairSteps", "FairID", "dbo.Fairs");
            DropForeignKey("dbo.Articles", "CourseID", "dbo.Courses");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.SaleItems", new[] { "Sale_Id" });
            DropIndex("dbo.SaleItems", new[] { "OfferID" });
            DropIndex("dbo.Sales", new[] { "SellerID" });
            DropIndex("dbo.Sales", new[] { "FairID" });
            DropIndex("dbo.OfferImages", new[] { "Offer_Id" });
            DropIndex("dbo.Offers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Offers", new[] { "Fair_Id" });
            DropIndex("dbo.Offers", new[] { "ArticleID" });
            DropIndex("dbo.FairSteps", new[] { "FairID" });
            DropIndex("dbo.Articles", new[] { "CourseID" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.SaleItems");
            DropTable("dbo.Sales");
            DropTable("dbo.OfferImages");
            DropTable("dbo.Offers");
            DropTable("dbo.FairSteps");
            DropTable("dbo.Fairs");
            DropTable("dbo.Courses");
            DropTable("dbo.Articles");
        }
    }
}
