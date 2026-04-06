using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OODProject
{
    internal class Features
    {
        [Key]

        public string index { get; set; }
        public string desc { get; set; }
    }
}
