namespace LivrETS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class delete_field_subtitle_article : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Articles", "SubTitle", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Articles", "SubTitle", c => c.String(nullable: false, maxLength: 256));
        }
    }
}
