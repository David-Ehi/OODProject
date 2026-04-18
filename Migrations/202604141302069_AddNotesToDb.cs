namespace OODProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNotesToDb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Characters", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Characters", "Notes");
        }
    }
}
