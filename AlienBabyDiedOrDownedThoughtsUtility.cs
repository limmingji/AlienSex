using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x02000458 RID: 1112
    public static class AlienBabyDiedOrDownedThoughtsUtility
    {
        // Token: 0x060012A8 RID: 4776 RVA: 0x0008F33C File Offset: 0x0008D73C
        public static void TryGiveThoughts(Pawn alienbaby, DamageInfo? dinfo, AlienBabyDiedOrDownedThoughtsKind thoughtsKind)
        {
            try
            {
                if (!PawnGenerator.IsBeingGenerated(alienbaby))
                {
                    if (Current.ProgramState == ProgramState.Playing)
                    {
                        AlienBabyDiedOrDownedThoughtsUtility.GetThoughts(alienbaby, dinfo, thoughtsKind, AlienBabyDiedOrDownedThoughtsUtility.tmpIndividualThoughtsToAdd, AlienBabyDiedOrDownedThoughtsUtility.tmpAllColonistsThoughts);
                        for (int i = 0; i < AlienBabyDiedOrDownedThoughtsUtility.tmpIndividualThoughtsToAdd.Count; i++)
                        {
                            AlienBabyDiedOrDownedThoughtsUtility.tmpIndividualThoughtsToAdd[i].Add();
                        }
                        if (AlienBabyDiedOrDownedThoughtsUtility.tmpAllColonistsThoughts.Any<ThoughtDef>())
                        {
                            foreach (Pawn pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Colonists)
                            {
                                if (pawn != alienbaby)
                                {
                                    for (int j = 0; j < AlienBabyDiedOrDownedThoughtsUtility.tmpAllColonistsThoughts.Count; j++)
                                    {
                                        ThoughtDef def = AlienBabyDiedOrDownedThoughtsUtility.tmpAllColonistsThoughts[j];
                                        pawn.needs.mood.thoughts.memories.TryGainMemory(def, null);
                                    }
                                }
                            }
                        }
                        AlienBabyDiedOrDownedThoughtsUtility.tmpIndividualThoughtsToAdd.Clear();
                        AlienBabyDiedOrDownedThoughtsUtility.tmpAllColonistsThoughts.Clear();
                    }
                }
            }
            catch (Exception arg)
            {
                Log.Error("Could not give thoughts: " + arg);
            }
        }

        // Token: 0x060012A9 RID: 4777 RVA: 0x0008F4A8 File Offset: 0x0008D8A8
        public static void TryGiveThoughts(IEnumerable<Pawn> alienbabies, AlienBabyDiedOrDownedThoughtsKind thoughtsKind)
        {
            foreach (Pawn alienbaby in alienbabies)
            {
                AlienBabyDiedOrDownedThoughtsUtility.TryGiveThoughts(alienbaby, null, thoughtsKind);
            }
        }

        // Token: 0x060012AA RID: 4778 RVA: 0x0008F508 File Offset: 0x0008D908
        public static void GetThoughts(Pawn alienbaby, DamageInfo? dinfo, AlienBabyDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtDef> outAllColonistsThoughts)
        {
            outIndividualThoughts.Clear();
            outAllColonistsThoughts.Clear();
            if (alienbaby.relations == null)
            {
                AlienBabyDiedOrDownedThoughtsUtility.AppendThoughts_ForHumanlike(alienbaby, dinfo, thoughtsKind, outIndividualThoughts, outAllColonistsThoughts);
            }
            if (alienbaby.relations != null && alienbaby.relations.everSeenByPlayer)
            {
                AlienBabyDiedOrDownedThoughtsUtility.AppendThoughts_Relations(alienbaby, dinfo, thoughtsKind, outIndividualThoughts, outAllColonistsThoughts);
            }
        }


        // Token: 0x060012AB RID: 4779 RVA: 0x0008F564 File Offset: 0x0008D964
        public static void BuildMoodThoughtsListString(Pawn alienbaby, DamageInfo? dinfo, AlienBabyDiedOrDownedThoughtsKind thoughtsKind, StringBuilder sb, string individualThoughtsHeader, string allColonistsThoughtsHeader)
        {
            AlienBabyDiedOrDownedThoughtsUtility.GetThoughts(alienbaby, dinfo, thoughtsKind, AlienBabyDiedOrDownedThoughtsUtility.tmpIndividualThoughtsToAdd, AlienBabyDiedOrDownedThoughtsUtility.tmpAllColonistsThoughts);
            if (AlienBabyDiedOrDownedThoughtsUtility.tmpAllColonistsThoughts.Any<ThoughtDef>())
            {
                if (!allColonistsThoughtsHeader.NullOrEmpty())
                {
                    sb.Append(allColonistsThoughtsHeader);
                    sb.AppendLine();
                }
                for (int i = 0; i < AlienBabyDiedOrDownedThoughtsUtility.tmpAllColonistsThoughts.Count; i++)
                {
                    ThoughtDef thoughtDef = AlienBabyDiedOrDownedThoughtsUtility.tmpAllColonistsThoughts[i];
                    if (sb.Length > 0)
                    {
                        sb.AppendLine();
                    }
                    sb.Append("  - " + thoughtDef.stages[0].label.CapitalizeFirst() + " " + Mathf.RoundToInt(thoughtDef.stages[0].baseMoodEffect).ToStringWithSign());
                }
            }
            if (AlienBabyDiedOrDownedThoughtsUtility.tmpIndividualThoughtsToAdd.Any((IndividualThoughtToAdd x) => x.thought.MoodOffset() != 0f))
            {
                if (!individualThoughtsHeader.NullOrEmpty())
                {
                    sb.Append(individualThoughtsHeader);
                }
                foreach (IGrouping<Pawn, IndividualThoughtToAdd> grouping in from x in AlienBabyDiedOrDownedThoughtsUtility.tmpIndividualThoughtsToAdd
                                                                             where x.thought.MoodOffset() != 0f
                                                                             group x by x.addTo)
                {
                    if (sb.Length > 0)
                    {
                        sb.AppendLine();
                        sb.AppendLine();
                    }
                    string value = grouping.Key.KindLabel.CapitalizeFirst() + " " + grouping.Key.LabelShort;
                    sb.Append(value);
                    sb.Append(":");
                    foreach (IndividualThoughtToAdd individualThoughtToAdd in grouping)
                    {
                        sb.AppendLine();
                        sb.Append("    " + individualThoughtToAdd.LabelCap);
                    }
                }
            }
        }

        // Token: 0x060012AC RID: 4780 RVA: 0x0008F7B0 File Offset: 0x0008DBB0
        public static void BuildMoodThoughtsListString(IEnumerable<Pawn> alienbabies, AlienBabyDiedOrDownedThoughtsKind thoughtsKind, StringBuilder sb, string individualThoughtsHeader, string allColonistsThoughtsHeader, string victimLabelKey)
        {
            foreach (Pawn pawn in alienbabies)
            {
                AlienBabyDiedOrDownedThoughtsUtility.GetThoughts(pawn, null, thoughtsKind, AlienBabyDiedOrDownedThoughtsUtility.tmpIndividualThoughtsToAdd, AlienBabyDiedOrDownedThoughtsUtility.tmpAllColonistsThoughts);
                if (AlienBabyDiedOrDownedThoughtsUtility.tmpIndividualThoughtsToAdd.Any<IndividualThoughtToAdd>() || AlienBabyDiedOrDownedThoughtsUtility.tmpAllColonistsThoughts.Any<ThoughtDef>())
                {
                    if (sb.Length > 0)
                    {
                        sb.AppendLine();
                        sb.AppendLine();
                    }
                    string text = pawn.KindLabel.CapitalizeFirst() + " " + pawn.LabelShort;
                    if (victimLabelKey.NullOrEmpty())
                    {
                        sb.Append(text + ":");
                    }
                    else
                    {
                        sb.Append(victimLabelKey.Translate(new object[]
                        {
                            text
                        }));
                    }
                    AlienBabyDiedOrDownedThoughtsUtility.BuildMoodThoughtsListString(pawn, null, thoughtsKind, sb, individualThoughtsHeader, allColonistsThoughtsHeader);
                }
            }
        }

        // Token: 0x060012AD RID: 4781 RVA: 0x0008F8C0 File Offset: 0x0008DCC0
        private static void AppendThoughts_ForHumanlike(Pawn alienbaby, DamageInfo? dinfo, AlienBabyDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtDef> outAllColonistsThoughts)
        {
            bool flag = dinfo != null && dinfo.Value.Def.execution;
            bool flag2 = alienbaby.IsPrisonerOfColony && !alienbaby.guilt.IsGuilty && !alienbaby.InAggroMentalState;
            bool flag3 = dinfo != null && dinfo.Value.Def.externalViolence && dinfo.Value.Instigator != null && dinfo.Value.Instigator is Pawn;
            bool flag4 = !alienbaby.health.Downed && !alienbaby.health.Dead && alienbaby.health.summaryHealth.SummaryHealthPercent == 1f;
            if (flag4)
            {
                Pawn pawn = (Pawn)dinfo.Value.Instigator;
                outIndividualThoughts.Add(new IndividualThoughtToAdd(AlienSexThoughtDefOf.SeeAlienBaby, pawn, alienbaby, 1f, 1f));
            }
            if (flag3)
            {
                Pawn pawn = (Pawn)dinfo.Value.Instigator;
                if (!pawn.Dead && pawn.needs.mood != null && pawn.story != null && pawn != alienbaby)
                {
                    if (thoughtsKind == AlienBabyDiedOrDownedThoughtsKind.Died)
                    {
                        outIndividualThoughts.Add(new IndividualThoughtToAdd(ThoughtDefOf.KilledHumanlikeBloodlust, pawn, null, 1f, 1f));
                    }
                    if (thoughtsKind == AlienBabyDiedOrDownedThoughtsKind.Died && alienbaby.HostileTo(pawn))
                    {
                        if (alienbaby.Faction != null && PawnUtility.IsFactionLeader(alienbaby) && alienbaby.Faction.HostileTo(pawn.Faction))
                        {
                            outIndividualThoughts.Add(new IndividualThoughtToAdd(ThoughtDefOf.DefeatedHostileFactionLeader, pawn, alienbaby, 1f, 1f));
                        }
                        if (alienbaby.kindDef.combatPower > 250f)
                        {
                            outIndividualThoughts.Add(new IndividualThoughtToAdd(ThoughtDefOf.DefeatedMajorEnemy, pawn, alienbaby, 1f, 1f));
                        }
                    }
                }
            }
            if (thoughtsKind == AlienBabyDiedOrDownedThoughtsKind.Died && !flag)
            {
                foreach (Pawn pawn2 in PawnsFinder.AllMapsCaravansAndTravelingTransportPods)
                {
                    if (pawn2 != alienbaby && pawn2.needs.mood != null)
                    {
                        if (pawn2.MentalStateDef != MentalStateDefOf.SocialFighting || ((MentalState_SocialFighting)pawn2.MentalState).otherPawn != alienbaby)
                        {
                            if (AlienBabyDiedOrDownedThoughtsUtility.Witnessed(pawn2, alienbaby))
                            {
                                if (pawn2.Faction == alienbaby.Faction)
                                {
                                    outIndividualThoughts.Add(new IndividualThoughtToAdd(ThoughtDefOf.WitnessedDeathAlly, pawn2, null, 1f, 1f));
                                }
                                else if (alienbaby.Faction == null || !alienbaby.Faction.HostileTo(pawn2.Faction))
                                {
                                    outIndividualThoughts.Add(new IndividualThoughtToAdd(ThoughtDefOf.WitnessedDeathNonAlly, pawn2, null, 1f, 1f));
                                }
                                if (pawn2.relations.FamilyByBlood.Contains(alienbaby))
                                {
                                    outIndividualThoughts.Add(new IndividualThoughtToAdd(ThoughtDefOf.WitnessedDeathFamily, pawn2, null, 1f, 1f));
                                }
                                outIndividualThoughts.Add(new IndividualThoughtToAdd(ThoughtDefOf.WitnessedDeathBloodlust, pawn2, null, 1f, 1f));
                            }
                            else if (alienbaby.Faction == Faction.OfPlayer && alienbaby.Faction == pawn2.Faction && alienbaby.HostFaction != pawn2.Faction)
                            {
                                outIndividualThoughts.Add(new IndividualThoughtToAdd(ThoughtDefOf.KnowColonistDied, pawn2, null, 1f, 1f));
                            }
                            if (flag2 && pawn2.Faction == Faction.OfPlayer && !pawn2.IsPrisoner)
                            {
                                outIndividualThoughts.Add(new IndividualThoughtToAdd(ThoughtDefOf.KnowPrisonerDiedInnocent, pawn2, null, 1f, 1f));
                            }
                        }
                    }
                }
            }
            if (thoughtsKind == AlienBabyDiedOrDownedThoughtsKind.Banished && alienbaby.IsColonist)
            {
                outAllColonistsThoughts.Add(ThoughtDefOf.ColonistBanished);
            }
            if (thoughtsKind == AlienBabyDiedOrDownedThoughtsKind.BanishedToDie)
            {
                if (alienbaby.IsColonist)
                {
                    outAllColonistsThoughts.Add(ThoughtDefOf.ColonistBanishedToDie);
                }
                else if (alienbaby.IsPrisonerOfColony)
                {
                    outAllColonistsThoughts.Add(ThoughtDefOf.PrisonerBanishedToDie);
                }
            }
        }

        // Token: 0x060012AE RID: 4782 RVA: 0x0008FCE0 File Offset: 0x0008E0E0
        private static void AppendThoughts_Relations(Pawn alienbaby, DamageInfo? dinfo, AlienBabyDiedOrDownedThoughtsKind thoughtsKind, List<IndividualThoughtToAdd> outIndividualThoughts, List<ThoughtDef> outAllColonistsThoughts)
        {
            if (alienbaby.health.summaryHealth.SummaryHealthPercent == 1f)
            {
                Pawn pawn = (Pawn)dinfo.Value.Instigator;
                outIndividualThoughts.Add(new IndividualThoughtToAdd(AlienSexThoughtDefOf.SeeAlienBaby, pawn, alienbaby, 1f, 1f));
            }

            if (thoughtsKind == AlienBabyDiedOrDownedThoughtsKind.Banished && alienbaby.RaceProps.Animal)
            {
                List<DirectPawnRelation> directRelations = alienbaby.relations.DirectRelations;
                for (int i = 0; i < directRelations.Count; i++)
                {
                    if (!directRelations[i].otherPawn.Dead && directRelations[i].otherPawn.needs.mood != null)
                    {
                        if (PawnUtility.ShouldGetThoughtAbout(directRelations[i].otherPawn, alienbaby))
                        {
                            if (directRelations[i].def == PawnRelationDefOf.Bond)
                            {
                                outIndividualThoughts.Add(new IndividualThoughtToAdd(ThoughtDefOf.BondedAnimalBanished, directRelations[i].otherPawn, alienbaby, 1f, 1f));
                            }
                        }
                    }
                }
            }
            if (thoughtsKind == AlienBabyDiedOrDownedThoughtsKind.Died || thoughtsKind == AlienBabyDiedOrDownedThoughtsKind.BanishedToDie)
            {
                foreach (Pawn pawn in alienbaby.relations.PotentiallyRelatedPawns)
                {
                    if (!pawn.Dead && pawn.needs.mood != null)
                    {
                        if (PawnUtility.ShouldGetThoughtAbout(pawn, alienbaby))
                        {
                            PawnRelationDef mostImportantRelation = pawn.GetMostImportantRelation(alienbaby);
                            if (mostImportantRelation != null)
                            {
                                ThoughtDef genderSpecificDiedThought = mostImportantRelation.GetGenderSpecificDiedThought(alienbaby);
                                if (genderSpecificDiedThought != null)
                                {
                                    outIndividualThoughts.Add(new IndividualThoughtToAdd(genderSpecificDiedThought, pawn, alienbaby, 1f, 1f));
                                }
                            }
                        }
                    }
                }
                if (dinfo != null)
                {
                    Pawn pawn2 = dinfo.Value.Instigator as Pawn;
                    if (pawn2 != null && pawn2 != alienbaby)
                    {
                        foreach (Pawn pawn3 in alienbaby.relations.PotentiallyRelatedPawns)
                        {
                            if (pawn2 != pawn3 && !pawn3.Dead && pawn3.needs.mood != null)
                            {
                                PawnRelationDef mostImportantRelation2 = pawn3.GetMostImportantRelation(alienbaby);
                                if (mostImportantRelation2 != null)
                                {
                                    ThoughtDef genderSpecificKilledThought = mostImportantRelation2.GetGenderSpecificKilledThought(alienbaby);
                                    if (genderSpecificKilledThought != null)
                                    {
                                        outIndividualThoughts.Add(new IndividualThoughtToAdd(genderSpecificKilledThought, pawn3, pawn2, 1f, 1f));
                                    }
                                }
                                if (pawn3.RaceProps.IsFlesh)
                                {
                                    int num = pawn3.relations.OpinionOf(alienbaby);
                                    if (num >= 20)
                                    {
                                        ThoughtDef thoughtDef = ThoughtDefOf.KilledMyFriend;
                                        Pawn pawn4 = pawn3;
                                        Pawn pawn5 = pawn2;
                                        float opinionOffsetFactor = alienbaby.relations.GetFriendDiedThoughtPowerFactor(num);
                                        outIndividualThoughts.Add(new IndividualThoughtToAdd(thoughtDef, pawn4, pawn5, 1f, opinionOffsetFactor));
                                    }
                                    else if (num <= -20)
                                    {
                                        ThoughtDef thoughtDef = ThoughtDefOf.KilledMyRival;
                                        Pawn pawn5 = pawn3;
                                        Pawn pawn4 = pawn2;
                                        float opinionOffsetFactor = alienbaby.relations.GetRivalDiedThoughtPowerFactor(num);
                                        outIndividualThoughts.Add(new IndividualThoughtToAdd(thoughtDef, pawn5, pawn4, 1f, opinionOffsetFactor));
                                    }
                                }
                            }
                        }
                    }
                }
                if (alienbaby.RaceProps.Humanlike)
                {
                    foreach (Pawn pawn6 in PawnsFinder.AllMapsCaravansAndTravelingTransportPods)
                    {
                        if (!pawn6.Dead && pawn6.RaceProps.IsFlesh && pawn6.needs.mood != null)
                        {
                            if (PawnUtility.ShouldGetThoughtAbout(pawn6, alienbaby))
                            {
                                int num2 = pawn6.relations.OpinionOf(alienbaby);
                                if (num2 >= 20)
                                {
                                    outIndividualThoughts.Add(new IndividualThoughtToAdd(ThoughtDefOf.PawnWithGoodOpinionDied, pawn6, alienbaby, alienbaby.relations.GetFriendDiedThoughtPowerFactor(num2), 1f));
                                }
                                else if (num2 <= -20)
                                {
                                    outIndividualThoughts.Add(new IndividualThoughtToAdd(ThoughtDefOf.PawnWithBadOpinionDied, pawn6, alienbaby, alienbaby.relations.GetRivalDiedThoughtPowerFactor(num2), 1f));
                                }
                            }
                        }
                    }
                }
            }
        }

        // Token: 0x060012AF RID: 4783 RVA: 0x00090130 File Offset: 0x0008E530
        private static bool Witnessed(Pawn p, Pawn alienbaby)
        {
            if (!p.Awake() || !p.health.capacities.CapableOf(PawnCapacityDefOf.Sight) || alienbaby.ageTracker.CurLifeStageIndex > 0 || alienbaby.RaceProps.Animal)
            {
                return false;
            }
            if (alienbaby.IsCaravanMember())
            {
                return alienbaby.GetCaravan() == p.GetCaravan();
            }
            return alienbaby.Spawned && p.Spawned && p.Position.InHorDistOf(alienbaby.Position, 12f) && GenSight.LineOfSight(alienbaby.Position, p.Position, alienbaby.Map, false, null, 0, 0);
        }

        // Token: 0x04000B6A RID: 2922
        private static List<IndividualThoughtToAdd> tmpIndividualThoughtsToAdd = new List<IndividualThoughtToAdd>();

        // Token: 0x04000B6B RID: 2923
        private static List<ThoughtDef> tmpAllColonistsThoughts = new List<ThoughtDef>();
    }
}
