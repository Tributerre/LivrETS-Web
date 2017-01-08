namespace LivrETS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtablefairstep : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FairSteps",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        Place = c.String(nullable: false),
                        StartDateTime = c.DateTime(nullable: false),
                        EndDateTime = c.DateTime(nullable: false),
                        Fair_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Fairs", t => t.Fair_Id)
                .Index(t => t.Fair_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FairSteps", "Fair_Id", "dbo.Fairs");
            DropIndex("dbo.FairSteps", new[] { "Fair_Id" });
            DropTable("dbo.FairSteps");
        }
    }
}
