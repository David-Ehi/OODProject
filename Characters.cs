using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OODProject
{
    public class Characters
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Class { get; set; }

        public int Level { get; set; }

        public int Strength { get; set; }
        public int Constitution { get; set; }
        public int Wisdom { get; set; }
        public int Intellegence { get; set; }
        public int Dexterity { get; set; }
        public int Charisma { get; set; }
        public int HP { get; set; }
        public int AC { get; set; }
        public Characters() { }

        public override string ToString()
        {
            return Name;
        }

    }
}
