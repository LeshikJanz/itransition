namespace MyAmdm.Migrations
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
                        Img64 = c.String(),
                    })
                .PrimaryKey(t => t.AccordId);
            
            CreateTable(
                "dbo.Authors",
                c => new
                    {
                        AuthorId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Biography = c.String(),
                    })
                .PrimaryKey(t => t.AuthorId);
            
            CreateTable(
                "dbo.Songs",
                c => new
                    {
                        SongId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        lyric = c.String(),
                    })
                .PrimaryKey(t => t.SongId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Songs");
            DropTable("dbo.Authors");
            DropTable("dbo.Accords");
        }
    }
}
