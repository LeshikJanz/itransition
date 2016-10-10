namespace AmdmProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedaccordLinkfieldtoSongName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accords", "SongName", c => c.String());
            DropColumn("dbo.Accords", "AccordLink");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Accords", "AccordLink", c => c.String());
            DropColumn("dbo.Accords", "SongName");
        }
    }
}
