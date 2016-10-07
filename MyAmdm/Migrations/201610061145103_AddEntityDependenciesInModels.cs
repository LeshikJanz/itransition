namespace MyAmdm.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEntityDependenciesInModels : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Songs");
            AlterColumn("dbo.Songs", "SongId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Songs", "AuthorId");
            CreateIndex("dbo.Songs", "AuthorId");
            AddForeignKey("dbo.Songs", "AuthorId", "dbo.Authors", "AuthorId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Songs", "AuthorId", "dbo.Authors");
            DropIndex("dbo.Songs", new[] { "AuthorId" });
            DropPrimaryKey("dbo.Songs");
            AlterColumn("dbo.Songs", "SongId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Songs", "SongId");
        }
    }
}
