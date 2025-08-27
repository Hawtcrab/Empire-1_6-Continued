using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace FactionColonies
{
    /// <summary>
    /// Maps a pawn trait to governor personality values. Generally, values should range from -2 to +2.
    /// The axis are, from negative values to positive:
    /// Cruel/Kind
    /// Erratic/Rational
    /// Decadent/Diligent
    /// </summary>
    public class GovTraitDef : Def, IExposable
    {
        public void ExposeData()
        {
            Scribe_Deep.Look(ref trait, "trait");
            Scribe_Values.Look(ref kind, "kind");
            Scribe_Values.Look(ref rational, "rational");
            Scribe_Values.Look(ref diligent, "diligent");

        }

        TraitDef trait;
        int kind = 0;
        int rational = 0;
        int diligent = 0;
        TraitDefOf

        
    }
}
