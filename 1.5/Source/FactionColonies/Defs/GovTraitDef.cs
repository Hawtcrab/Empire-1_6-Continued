using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FactionColonies
{
    public class GovTraitDef : Def
    {
        public GovTraitDef() { }

        public TraitDef trait;
        public int kind = 0;
        public int rational = 0;
        public int professional = 0;
        public int likeable = 0;
        public int degree = 0;
    }
}
