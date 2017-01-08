namespace LivrETS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetablefair : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Fairs", "PickingStartDate");
            DropColumn("dbo.Fairs", "PickingEndDate");
            DropColumn("dbo.Fairs", "SaleStartDate");
            DropColumn("dbo.Fairs", "SaleEndDate");
            DropColumn("dbo.Fairs", "RetrievalStartDate");
            DropColumn("dbo.Fairs", "RetrievalEndDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Fairs", "RetrievalEndDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Fairs", "RetrievalStartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Fairs", "SaleEndDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Fairs", "SaleStartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Fairs", "PickingEndDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Fairs", "PickingStartDate", c => c.DateTime(nullable: false));
        }
    }
}
