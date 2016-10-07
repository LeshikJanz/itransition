namespace MyAmdm.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrateDB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Songs", "AuthorId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Songs", "AuthorId");
        }
    }
}
