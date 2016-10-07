namespace MyAmdm.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddrelationSongIdtoAccordmodelforconnectingwithSongmodel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SongAccords", "Accord_AccordId", "dbo.Accords");
            RenameColumn(table: "dbo.SongAccords", name: "Accord_AccordId", newName: "Accord_SongId");
            RenameIndex(table: "dbo.SongAccords", name: "IX_Accord_AccordId", newName: "IX_Accord_SongId");
            DropPrimaryKey("dbo.Accords");
            AddColumn("dbo.Accords", "SongId", c => c.Int(nullable: false));
            AlterColumn("dbo.Accords", "AccordId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Accords", "SongId");
            CreateIndex("dbo.Accords", "SongId");
            AddForeignKey("dbo.Accords", "SongId", "dbo.Authors", "AuthorId");
            AddForeignKey("dbo.SongAccords", "Accord_SongId", "dbo.Accords", "SongId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SongAccords", "Accord_SongId", "dbo.Accords");
            DropForeignKey("dbo.Accords", "SongId", "dbo.Authors");
            DropIndex("dbo.Accords", new[] { "SongId" });
            DropPrimaryKey("dbo.Accords");
            AlterColumn("dbo.Accords", "AccordId", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.Accords", "SongId");
            AddPrimaryKey("dbo.Accords", "AccordId");
            RenameIndex(table: "dbo.SongAccords", name: "IX_Accord_SongId", newName: "IX_Accord_AccordId");
            RenameColumn(table: "dbo.SongAccords", name: "Accord_SongId", newName: "Accord_AccordId");
            AddForeignKey("dbo.SongAccords", "Accord_AccordId", "dbo.Accords", "AccordId", cascadeDelete: true);
        }
    }
}
