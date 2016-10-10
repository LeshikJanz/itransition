namespace AmdmProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accords",
                c => new
                    {
                        AccordId = c.Int(nullable: false, identity: true),
                        AccordName = c.String(),
                        Img64 = c.String(),
                    })
                .PrimaryKey(t => t.AccordId);
            
            CreateTable(
                "dbo.Songs",
                c => new
                    {
                        SongId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        lyric = c.String(),
                        LinkOfSong = c.String(),
                        NumberOfView = c.Int(nullable: false),
                        AuthorId = c.Int(),
                    })
                .PrimaryKey(t => t.SongId)
                .ForeignKey("dbo.Authors", t => t.AuthorId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.Authors",
                c => new
                    {
                        AuthorId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Biography = c.String(),
                        LinkOfBiography = c.String(),
                    })
                .PrimaryKey(t => t.AuthorId);
            
            CreateTable(
                "dbo.SongAccords",
                c => new
                    {
                        Song_SongId = c.Int(nullable: false),
                        Accord_AccordId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Song_SongId, t.Accord_AccordId })
                .ForeignKey("dbo.Songs", t => t.Song_SongId, cascadeDelete: true)
                .ForeignKey("dbo.Accords", t => t.Accord_AccordId, cascadeDelete: true)
                .Index(t => t.Song_SongId)
                .Index(t => t.Accord_AccordId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Songs", "AuthorId", "dbo.Authors");
            DropForeignKey("dbo.SongAccords", "Accord_AccordId", "dbo.Accords");
            DropForeignKey("dbo.SongAccords", "Song_SongId", "dbo.Songs");
            DropIndex("dbo.SongAccords", new[] { "Accord_AccordId" });
            DropIndex("dbo.SongAccords", new[] { "Song_SongId" });
            DropIndex("dbo.Songs", new[] { "AuthorId" });
            DropTable("dbo.SongAccords");
            DropTable("dbo.Authors");
            DropTable("dbo.Songs");
            DropTable("dbo.Accords");
        }
    }
}
