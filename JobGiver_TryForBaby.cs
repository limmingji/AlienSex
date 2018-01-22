using System;
using Verse;
using Verse.AI;


namespace RimWorld
{
    public class JobGiver_TryForBaby : ThinkNode_JobGiver
    {
        [DefOf]
        public static class AlienSexJobDefOf
        {
            public static JobDef TryForBaby;
        }
        
        protected override Job TryGiveJob(Pawn pawn)
        {
            float ageMin = pawn.kindDef.race.GetStatValueAbstract(AlienSexStatDefOf.SexMaturityAgeMin);
            float ageMax = pawn.kindDef.race.GetStatValueAbstract(AlienSexStatDefOf.SexMaturityAgeMax);
            if (pawn.ageTracker.AgeBiologicalYearsFloat < ageMin || pawn.ageTracker.AgeBiologicalYearsFloat > ageMax)
            {
                Log.Message(pawn + "is not old enough to try for baby");
                return null;
            }            
            if (Find.TickManager.TicksGame < pawn.mindState.canLovinTick)
            {
                return null;
            }
            if (pawn.CurrentBed() == null || pawn.CurrentBed().Medical || !pawn.health.capacities.CanBeAwake)
            {
                return null;
            }
            Pawn partnerInMyBed = LovePartnerRelationUtility.GetPartnerInMyBed(pawn);
            
            if (partnerInMyBed == null || !partnerInMyBed.health.capacities.CanBeAwake || Find.TickManager.TicksGame < partnerInMyBed.mindState.canLovinTick)
            {
                return null;
            }
            if (!pawn.CanReserve(partnerInMyBed, 1, -1, null, false) || !partnerInMyBed.CanReserve(pawn, 1, -1, null, false))
            {
                return null;
            }
            if (pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Pregnant, true) != null || partnerInMyBed.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Pregnant, true) != null)
            {
                return new Job(JobDefOf.Lovin, partnerInMyBed, pawn.CurrentBed());
            }
            float ageMin2 = partnerInMyBed.kindDef.race.GetStatValueAbstract(AlienSexStatDefOf.SexMaturityAgeMin);
            float ageMax2 = partnerInMyBed.kindDef.race.GetStatValueAbstract(AlienSexStatDefOf.SexMaturityAgeMax);
            if (partnerInMyBed.ageTracker.AgeBiologicalYearsFloat < ageMin2 || partnerInMyBed.ageTracker.AgeBiologicalYearsFloat > ageMax2)
            {
                Log.Message(partnerInMyBed + "is not old enough to try for baby");
                return null;
            }
            if (pawn.TryGetComp<AlienSexTracker>() == null && partnerInMyBed.TryGetComp<AlienSexTracker>() == null)
            {
                //Log.Message("Both without tracker - Pawn: " + pawn + "  Partner: " + partnerInMyBed);
                return new Job(AlienSexJobDefOf.TryForBaby, partnerInMyBed, pawn.CurrentBed());
            }
            if (pawn.TryGetComp<AlienSexTracker>() != null)
            {
                if (partnerInMyBed.TryGetComp<AlienSexTracker>() != null)
                {
                    if (!pawn.TryGetComp<AlienSexTracker>().wantAlienBaby || !partnerInMyBed.TryGetComp<AlienSexTracker>().wantAlienBaby)
                    {
                        //Log.Message("Someone dun want - Pawn: " + pawn + "  Partner: " + partnerInMyBed);
                        return new Job(JobDefOf.Lovin, partnerInMyBed, pawn.CurrentBed());
                    }

                    //Log.Message("Both Willing - Pawn: " + pawn + "  Partner: " + partnerInMyBed);
                    return new Job(AlienSexJobDefOf.TryForBaby, partnerInMyBed, pawn.CurrentBed());
                }
                if (!pawn.TryGetComp<AlienSexTracker>().wantAlienBaby)
                {
                    //Log.Message("Someone dun want - Pawn: " + pawn + "  Partner: " + partnerInMyBed);
                    return new Job(JobDefOf.Lovin, partnerInMyBed, pawn.CurrentBed());
                }
                //Log.Message("Willing + no opinion: " + pawn + "  Partner: " + partnerInMyBed);
                return new Job(AlienSexJobDefOf.TryForBaby, partnerInMyBed, pawn.CurrentBed());
            }
            //Log.Message("The end: " + pawn + "  Partner: " + partnerInMyBed);
            return null;
        }
    }
}
