using RimWorld.Planet;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using FactionColonies.util;

namespace FactionColonies
{
    public class Governor : IExposable
    {
        public SettlementFC settlement;
        public Pawn currentGovernor;

        public int kindValue = 0;
        public int professionalValue = 0;
        public int likeableValue = 0;
        public int rationalValue = 0;


        public Governor() { }
        public Governor(SettlementFC settlement)
        {
            this.settlement = settlement;
            updatePersonalityValues();
        }

        public void InstallGovernor(Pawn gov = null)
        {
            if (currentGovernor != null)
            {
                RemoveGovernor();
            }
            if (gov == null)
                this.currentGovernor = generateLocalGovernor();
            else
                this.currentGovernor = gov;
            settlement.updateProfitAndProduction();
            updatePersonalityValues();

        }

        public void RemoveGovernor()
        {
            if (currentGovernor == null) return;
            Find.WorldPawns.RemovePawn(currentGovernor);
            this.currentGovernor = null;
            settlement.updateProfitAndProduction();
            updatePersonalityValues();
        }

        private Pawn generateLocalGovernor()
        {
            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Colonist, FactionColonies.getPlayerColonyFaction(), context: PawnGenerationContext.NonPlayer));
            pawn.ideo.SetIdeo(settlement.Ideo());
            Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.KeepForever);
            pawn.skills.skills.ForEach(skill => skill.Level = Math.Min(skill.Level, 10));
            return pawn;
        }

        private void updatePersonalityValues()
        {
            if (this.currentGovernor == null)
            {
                rationalValue = 0; likeableValue = 0; kindValue = 0; professionalValue = 0;
                return;
            }

            var actualtraits = currentGovernor.story.traits.allTraits.Select(t => t.def);
            var govtraits = DefDatabase<GovTraitDef>.AllDefsListForReading.Where(t => actualtraits.Contains(t.trait));

            rationalValue = govtraits.Sum(t => t.rational); 
            likeableValue = govtraits.Sum(t => t.likeable); 
            kindValue = govtraits.Sum(t => t.kind); 
            professionalValue = govtraits.Sum(t => t.professional);
        }

        public string GetDescription()
        {
            if (currentGovernor == null) return "NoGovernor".Translate();

            var descParts = new List<string>();

            if (kindValue != 0) descParts.Add($"GovernorKind{strFromVal(kindValue)}".Translate());
            if (rationalValue != 0) descParts.Add($"GovernorRational{strFromVal(rationalValue)}".Translate());
            if (professionalValue != 0) descParts.Add($"GovernorProfessional{strFromVal(professionalValue)}".Translate());
            if (likeableValue != 0) descParts.Add($"GovernorLikeable{strFromVal(likeableValue)}".Translate());

            string desc = descParts.Count == 0
                ? "GovernorBoring".Translate()
                : "GovernorPersonality".Translate() + "\n" + string.Join("\n", descParts);

            return desc.Formatted(currentGovernor.Named("PAWN"));
        }

        private string strFromVal(int value)
        {
            if (value == 0) return null;
            if (value == 1) return "High";
            if (value > 1) return "VeryHigh";
            if (value < 0) return "Low";
             return "VeryLow";
        }

        public float getGovernorResourceMultiplier(ResourceType resourceType)
        {
            if (currentGovernor == null) return 0.33f;
            float relevantSkillRating = 0;
            var plants = currentGovernor.skills.GetSkill(SkillDefOf.Plants).Level;
            var intellectual = currentGovernor.skills.GetSkill(SkillDefOf.Intellectual).Level;
            var crafting = currentGovernor.skills.GetSkill(SkillDefOf.Crafting).Level;
            var construction = currentGovernor.skills.GetSkill(SkillDefOf.Construction).Level;
            var melee = currentGovernor.skills.GetSkill(SkillDefOf.Melee).Level;
            var ranged = currentGovernor.skills.GetSkill(SkillDefOf.Shooting).Level;
            var artistic = currentGovernor.skills.GetSkill(SkillDefOf.Artistic).Level;
            var social = currentGovernor.skills.GetSkill(SkillDefOf.Social).Level;
            var medical = currentGovernor.skills.GetSkill(SkillDefOf.Medicine).Level;
            var animals = currentGovernor.skills.GetSkill(SkillDefOf.Animals).Level;
            var mining = currentGovernor.skills.GetSkill(SkillDefOf.Mining).Level;
            switch (resourceType)
            {
                case ResourceType.Research: relevantSkillRating = intellectual; break;
                case ResourceType.Power: relevantSkillRating = intellectual; break;
                case ResourceType.Apparel: relevantSkillRating = crafting; break;
                case ResourceType.Weapons: relevantSkillRating = crafting; break;
                case ResourceType.Medicine: relevantSkillRating = medical; break;
                case ResourceType.Animals: relevantSkillRating = animals; break;
                case ResourceType.Food: relevantSkillRating = Math.Max(animals, plants); break;
                case ResourceType.Logging: relevantSkillRating = plants; break;
                case ResourceType.Mining: relevantSkillRating = mining; break;
            }
            float aptitude = (relevantSkillRating + social) * 0.0625f; // Leads to a total 2.5 multiplier if both social and the relevant ability are maxed.
            return Math.Max(aptitude, 0.5f); // The modifier is always at least 0.5, so combined ratings at 8 or below won't affect things unduly.
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref currentGovernor, "currentGovernor");
            Scribe_References.Look(ref settlement, "settlement");
        }
    }
}
