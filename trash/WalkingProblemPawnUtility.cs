using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
    // Token: 0x02000472 RID: 1138
    public static class WalkingProblemPawnUtility
    {
        [DefOf]
        public static class WalkingProblemHediffDefOf
        {
            // Token: 0x04001D46 RID: 7494
            public static HediffDef WalkingProblemPregnant;
        }

            // Token: 0x0600136F RID: 4975 RVA: 0x00097198 File Offset: 0x00095598
            public static void Mated(Pawn male, Pawn female)
        {
            if (!female.ageTracker.CurLifeStage.reproductive)
            {
                return;
            }
            CompEggLayer compEggLayer = female.TryGetComp<CompEggLayer>();
            if (compEggLayer != null)
            {
                compEggLayer.Fertilize(male);
            }
            else if (Rand.Value < 0.5f && !female.health.hediffSet.HasHediff(WalkingProblemHediffDefOf.WalkingProblemPregnant, false))
            {
                Hediff_WalkingProblemPregnant hediff_Pregnant = (Hediff_WalkingProblemPregnant)HediffMaker.MakeHediff(WalkingProblemHediffDefOf.WalkingProblemPregnant, female, null);
                hediff_Pregnant.father = male;
                female.health.AddHediff(hediff_Pregnant, null, null);
            }
        }

        // Token: 0x06001371 RID: 4977 RVA: 0x00097290 File Offset: 0x00095690
        public static bool TrySpawnHatchedOrBornPawn(Pawn pawn, Thing motherOrEgg)
        {
            if (motherOrEgg.SpawnedOrAnyParentSpawned)
            {
                return GenSpawn.Spawn(pawn, motherOrEgg.PositionHeld, motherOrEgg.MapHeld) != null;
            }
            Pawn pawn2 = motherOrEgg as Pawn;
            if (pawn2 != null)
            {
                if (pawn2.IsCaravanMember())
                {
                    pawn2.GetCaravan().AddPawn(pawn, true);
                    Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
                    return true;
                }
                if (pawn2.IsWorldPawn())
                {
                    Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
                    return true;
                }
            }
            else if (motherOrEgg.ParentHolder != null)
            {
                Pawn_InventoryTracker pawn_InventoryTracker = motherOrEgg.ParentHolder as Pawn_InventoryTracker;
                if (pawn_InventoryTracker != null)
                {
                    if (pawn_InventoryTracker.pawn.IsCaravanMember())
                    {
                        pawn_InventoryTracker.pawn.GetCaravan().AddPawn(pawn, true);
                        Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
                        return true;
                    }
                    if (pawn_InventoryTracker.pawn.IsWorldPawn())
                    {
                        Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Decide);
                        return true;
                    }
                }
            }
            return false;
        }
        
    }
}
