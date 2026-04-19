namespace OODProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubclass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Characters", "Subclass", c => c.String());
            AddColumn("dbo.Characters", "Background", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Characters", "Background");
            DropColumn("dbo.Characters", "Subclass");
        }
    }
}
