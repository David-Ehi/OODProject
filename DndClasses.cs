using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OODProject
{
    public class Result
    {
        public string index { get; set; }
        public string name { get; set; }
        public string url { get; set; }

        public override string ToString()
        {
            return name;
        }
    }

    public class Root
    {
        public int count { get; set; }
        public List<Result> results { get; set; }
    }


    public class SpellListRoot
    {
        public int count { get; set; }
        public List<SpellResult> results { get; set; }
    }

    public class SpellResult
    {
        public string index { get; set; }
        public string name { get; set; }
        public int level { get; set; }
        public string url { get; set; }

        public override string ToString() => name;
    }





}


