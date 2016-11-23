namespace LivrETS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfielddletedAtArticle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "DeletedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Articles", "DeletedAt");
        }
    }
}
