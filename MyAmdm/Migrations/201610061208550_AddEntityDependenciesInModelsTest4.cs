namespace MyAmdm.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEntityDependenciesInModelsTest4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accords", "AccordName", c => c.String());
            AddColumn("dbo.Songs", "NumberOfView", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Songs", "NumberOfView");
            DropColumn("dbo.Accords", "AccordName");
        }
    }
}
