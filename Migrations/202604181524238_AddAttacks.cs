namespace OODProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttacks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attacks",
                c => new
                    {
                        AttackId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        NumDice = c.Int(nullable: false),
                        DiceType = c.Int(nullable: false),
                        BonusDamage = c.Int(nullable: false),
                        ToHitBonus = c.Int(nullable: false),
                        CharacterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AttackId)
                .ForeignKey("dbo.Characters", t => t.CharacterId, cascadeDelete: true)
                .Index(t => t.CharacterId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Attacks", "CharacterId", "dbo.Characters");
            DropIndex("dbo.Attacks", new[] { "CharacterId" });
            DropTable("dbo.Attacks");
        }
    }
}
