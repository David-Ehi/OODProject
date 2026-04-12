namespace OODProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSpellcastingColumns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Characters", "CantripsKnown", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Characters", "CantripsKnown");
        }
    }
}
