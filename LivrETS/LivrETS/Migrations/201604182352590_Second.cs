namespace LivrETS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Second : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Fairs",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        PickingStartDate = c.DateTime(nullable: false),
                        PickingEndDate = c.DateTime(nullable: false),
                        SaleStartDate = c.DateTime(nullable: false),
                        SaleEndDate = c.DateTime(nullable: false),
                        RetrievalStartDate = c.DateTime(nullable: false),
                        RetrievalEndDate = c.DateTime(nullable: false),
                        Phase = c.Int(nullable: false),
                        Trimester = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Fairs");
        }
    }
}
