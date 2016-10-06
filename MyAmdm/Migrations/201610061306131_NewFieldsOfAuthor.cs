namespace MyAmdm.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewFieldsOfAuthor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Authors", "LinkOfBiography", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Authors", "LinkOfBiography");
        }
    }
}
