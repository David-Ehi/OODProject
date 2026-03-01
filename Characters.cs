using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public int Strength { get; set; }
        public int Constitution { get; set; }
        public int Wisdom { get; set; }
        public int Intelligence { get; set; } 
        public int Dexterity { get; set; }
        public int Charisma { get; set; }
        public int HP { get; set; }
        public int AC { get; set; }

        public virtual Player Player { get; set; }

        public override string ToString()
        {
            return Name;
        }
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
    }
}