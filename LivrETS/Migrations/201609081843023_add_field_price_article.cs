namespace LivrETS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_field_price_article : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Articles", "Price", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Articles", "Price");
        }
    }
}
