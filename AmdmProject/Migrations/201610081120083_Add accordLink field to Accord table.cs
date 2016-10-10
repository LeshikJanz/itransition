namespace AmdmProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddaccordLinkfieldtoAccordtable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accords", "AccordLink", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accords", "AccordLink");
        }
    }
}
