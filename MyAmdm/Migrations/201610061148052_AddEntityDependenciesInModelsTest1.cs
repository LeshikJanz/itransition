namespace MyAmdm.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEntityDependenciesInModelsTest1 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Accords");
            AddColumn("dbo.Accords", "SongId", c => c.Int(nullable: false));
            AlterColumn("dbo.Accords", "AccordId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Accords", "SongId");
            CreateIndex("dbo.Accords", "SongId");
            AddForeignKey("dbo.Accords", "SongId", "dbo.Songs", "AuthorId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Accords", "SongId", "dbo.Songs");
            DropIndex("dbo.Accords", new[] { "SongId" });
            DropPrimaryKey("dbo.Accords");
            AlterColumn("dbo.Accords", "AccordId", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.Accords", "SongId");
            AddPrimaryKey("dbo.Accords", "AccordId");
        }
    }
}
