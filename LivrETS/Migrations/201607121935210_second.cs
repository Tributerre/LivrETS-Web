namespace LivrETS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class second : DbMigration
    {
        public override void Up()
        {
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
            
            AddColumn("dbo.Fairs", "CommissionOnSale", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sales", "SellerID", "dbo.AspNetUsers");
            DropForeignKey("dbo.SaleItems", "Sale_Id", "dbo.Sales");
            DropForeignKey("dbo.SaleItems", "OfferID", "dbo.Offers");
            DropForeignKey("dbo.Sales", "FairID", "dbo.Fairs");
            DropIndex("dbo.SaleItems", new[] { "Sale_Id" });
            DropIndex("dbo.SaleItems", new[] { "OfferID" });
            DropIndex("dbo.Sales", new[] { "SellerID" });
            DropIndex("dbo.Sales", new[] { "FairID" });
            DropColumn("dbo.Fairs", "CommissionOnSale");
            DropTable("dbo.SaleItems");
            DropTable("dbo.Sales");
        }
    }
}
