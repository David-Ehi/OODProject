namespace OODProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRaces : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Characters", "Race", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Characters", "Race");
        }
    }
}
