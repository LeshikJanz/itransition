namespace MyAmdm.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEntityDependenciesInModelsTest2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Accords", "SongId", "dbo.Songs");
            DropIndex("dbo.Accords", new[] { "SongId" });
            DropPrimaryKey("dbo.Accords");
            CreateTable(
                "dbo.SongAccords",
                c => new
                    {
                        Song_AuthorId = c.Int(nullable: false),
                        Accord_AccordId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Song_AuthorId, t.Accord_AccordId })
                .ForeignKey("dbo.Songs", t => t.Song_AuthorId, cascadeDelete: true)
                .ForeignKey("dbo.Accords", t => t.Accord_AccordId, cascadeDelete: true)
                .Index(t => t.Song_AuthorId)
                .Index(t => t.Accord_AccordId);
            
            AlterColumn("dbo.Accords", "AccordId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Accords", "AccordId");
            DropColumn("dbo.Accords", "SongId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accords", "SongId", c => c.Int(nullable: false));
            DropForeignKey("dbo.SongAccords", "Accord_AccordId", "dbo.Accords");
            DropForeignKey("dbo.SongAccords", "Song_AuthorId", "dbo.Songs");
            DropIndex("dbo.SongAccords", new[] { "Accord_AccordId" });
            DropIndex("dbo.SongAccords", new[] { "Song_AuthorId" });
            DropPrimaryKey("dbo.Accords");
            AlterColumn("dbo.Accords", "AccordId", c => c.Int(nullable: false));
            DropTable("dbo.SongAccords");
            AddPrimaryKey("dbo.Accords", "SongId");
            CreateIndex("dbo.Accords", "SongId");
            AddForeignKey("dbo.Accords", "SongId", "dbo.Songs", "AuthorId");
        }
    }
}
