namespace OODProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCheckSpellCaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Characters", "IsSpellCaster", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Characters", "IsSpellCaster");
        }
    }
}
