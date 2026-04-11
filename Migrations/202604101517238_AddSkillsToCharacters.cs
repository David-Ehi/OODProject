namespace OODProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSkillsToCharacters : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Characters", "Skills", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Characters", "Skills");
        }
    }
}
