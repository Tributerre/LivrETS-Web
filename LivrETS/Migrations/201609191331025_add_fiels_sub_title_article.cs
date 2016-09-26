namespace LivrETS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_fiels_sub_title_article : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Articles", "SubTitle", c => c.String(nullable: false, maxLength: 256));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Articles", "SubTitle", c => c.String());
        }
    }
}
