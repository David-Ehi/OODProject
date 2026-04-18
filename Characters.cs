using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace OODProject
{
    public class Characters
    {
        [Key]
        public int CharacterId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Class { get; set; }
        public int Level { get; set; }
        public int ProficencyBonus { get; set; }

        public int Strength { get; set; }
        public int Constitution { get; set; }
        public int Wisdom { get; set; }
        public int Intelligence { get; set; }
        public int Dexterity { get; set; }
        public int Charisma { get; set; }

        public int HP { get; set; }
        public int AC { get; set; }

        public string Skills { get; set; }

        public string Notes { get; set; }

        
        public bool IsSpellCaster { get; set; }

        public int CantripsKnown { get; set; }
        public int SpellSlotsLevel1 { get; set; }
        public int SpellSlotsLevel2 { get; set; }
        public int SpellSlotsLevel3 { get; set; }
        public int SpellSlotsLevel4 { get; set; }
        public int SpellSlotsLevel5 { get; set; }
        public int SpellSlotsLevel6 { get; set; }
        public int SpellSlotsLevel7 { get; set; }
        public int SpellSlotsLevel8 { get; set; }
        public int SpellSlotsLevel9 { get; set; }

        
        public virtual ICollection<Spell> Spells { get; set; }

        public virtual ICollection<Attack> Attacks { get; set; }

        public Characters()
        {
            Attacks = new List<Attack>();
            Spells = new List<Spell>();
        }



        public virtual Player Player { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    
    public class Spell
    {
        [Key]
        public int SpellId { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }

        public string Description { get; set; }

        //link spell to characters
        public virtual ICollection<Characters> Characters { get; set; }

        public Spell()
        {
            Characters = new List<Characters>();
        }

    }

    public class Attack
    {
        [Key]
        public int AttackId { get; set; }
        public string Name { get; set; }
        public int NumDice { get; set; }
        public int DiceType { get; set; }
        public int BonusDamage { get; set; }
        public int ToHitBonus { get; set; }
        public int CharacterId { get; set; }
        public virtual Characters Character { get; set; }
    }



    public class Player
    {
        [Key]
        public int PlayerId { get; set; }

        public string Name { get; set; }

        public virtual List<Characters> Characters { get; set; }

        public Player()
        {
            Characters = new List<Characters>();
        }
    }

    public class PlayerData : DbContext
    {
        public PlayerData() : base("name=PlayerData") { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Characters> Characters { get; set; }
        public DbSet<Spell> Spells { get; set; }
        public DbSet<Attack> Attacks { get; set; }
    }


}