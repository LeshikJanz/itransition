namespace MyAmdm.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migrate3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Songs", "AuthorId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Songs", "AuthorId", c => c.String());
        }
    }
}
