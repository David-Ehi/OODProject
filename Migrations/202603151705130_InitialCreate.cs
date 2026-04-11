namespace OODProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Characters",
                c => new
                    {
                        CharacterId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Class = c.String(),
                        Level = c.Int(nullable: false),
                        ProficencyBonus = c.Int(nullable: false),
                        Strength = c.Int(nullable: false),
                        Constitution = c.Int(nullable: false),
                        Wisdom = c.Int(nullable: false),
                        Intelligence = c.Int(nullable: false),
                        Dexterity = c.Int(nullable: false),
                        Charisma = c.Int(nullable: false),
                        HP = c.Int(nullable: false),
                        AC = c.Int(nullable: false),
                        SpellSlotsLevel1 = c.Int(nullable: false),
                        SpellSlotsLevel2 = c.Int(nullable: false),
                        SpellSlotsLevel3 = c.Int(nullable: false),
                        SpellSlotsLevel4 = c.Int(nullable: false),
                        SpellSlotsLevel5 = c.Int(nullable: false),
                        SpellSlotsLevel6 = c.Int(nullable: false),
                        SpellSlotsLevel7 = c.Int(nullable: false),
                        SpellSlotsLevel8 = c.Int(nullable: false),
                        SpellSlotsLevel9 = c.Int(nullable: false),
                        Player_PlayerId = c.Int(),
                    })
                .PrimaryKey(t => t.CharacterId)
                .ForeignKey("dbo.Players", t => t.Player_PlayerId)
                .Index(t => t.Player_PlayerId);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        PlayerId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.PlayerId);
            
            CreateTable(
                "dbo.Spells",
                c => new
                    {
                        SpellId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Level = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.SpellId);
            
            CreateTable(
                "dbo.SpellCharacters",
                c => new
                    {
                        Spell_SpellId = c.Int(nullable: false),
                        Characters_CharacterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Spell_SpellId, t.Characters_CharacterId })
                .ForeignKey("dbo.Spells", t => t.Spell_SpellId, cascadeDelete: true)
                .ForeignKey("dbo.Characters", t => t.Characters_CharacterId, cascadeDelete: true)
                .Index(t => t.Spell_SpellId)
                .Index(t => t.Characters_CharacterId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SpellCharacters", "Characters_CharacterId", "dbo.Characters");
            DropForeignKey("dbo.SpellCharacters", "Spell_SpellId", "dbo.Spells");
            DropForeignKey("dbo.Characters", "Player_PlayerId", "dbo.Players");
            DropIndex("dbo.SpellCharacters", new[] { "Characters_CharacterId" });
            DropIndex("dbo.SpellCharacters", new[] { "Spell_SpellId" });
            DropIndex("dbo.Characters", new[] { "Player_PlayerId" });
            DropTable("dbo.SpellCharacters");
            DropTable("dbo.Spells");
            DropTable("dbo.Players");
            DropTable("dbo.Characters");
        }
    }
}
