using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;

namespace Verse
{
        // Token: 0x02000C7A RID: 3194
        public class Hediff_AlienPregnant : HediffWithComps
    {
        // Token: 0x17000AA0 RID: 2720
        // (get) Token: 0x0600435F RID: 17247 RVA: 0x001E7EF5 File Offset: 0x001E62F5
        // (set) Token: 0x06004360 RID: 17248 RVA: 0x001E7EFD File Offset: 0x001E62FD
        public float GestationProgress
        {
            get
            {
                return this.Severity;
            }
            private set
            {
                this.Severity = value;
            }
        }

        // Token: 0x17000AA1 RID: 2721
        // (get) Token: 0x06004361 RID: 17249 RVA: 0x001E7F08 File Offset: 0x001E6308
        private bool IsSeverelyWounded
        {
            get
            {
                float num = 0f;
                List<Hediff> hediffs = this.pawn.health.hediffSet.hediffs;
                for (int i = 0; i < hediffs.Count; i++)
                {
                    if (hediffs[i] is Hediff_Injury && !hediffs[i].IsOld())
                    {
                        num += hediffs[i].Severity;
                    }
                }
                List<Hediff_MissingPart> missingPartsCommonAncestors = this.pawn.health.hediffSet.GetMissingPartsCommonAncestors();
                for (int j = 0; j < missingPartsCommonAncestors.Count; j++)
                {
                    if (missingPartsCommonAncestors[j].IsFreshNonSolidExtremity)
                    {
                        num += missingPartsCommonAncestors[j].Part.def.GetMaxHealth(this.pawn);
                    }
                }
                return num > 38f * this.pawn.RaceProps.baseHealthScale;
            }
        }

        // Token: 0x06004362 RID: 17250 RVA: 0x001E7FF8 File Offset: 0x001E63F8
        public override void Tick()
        {
            this.ageTicks++;
            if (this.pawn.IsHashIntervalTick(1000))
            {
                if (this.pawn.needs.food != null && this.pawn.needs.food.CurCategory == HungerCategory.Starving && Rand.MTBEventOccurs(0.5f, 60000f, 1000f))
                {
                    if (this.Visible && PawnUtility.ShouldSendNotificationAbout(this.pawn))
                    {
                        Messages.Message("MessageMiscarriedStarvation".Translate(new object[]
                        {
                            this.pawn.LabelIndefinite()
                        }).CapitalizeFirst(), this.pawn, MessageTypeDefOf.NegativeHealthEvent);
                    }
                    this.Miscarry();
                    return;
                }
                if (this.IsSeverelyWounded && Rand.MTBEventOccurs(0.5f, 60000f, 1000f))
                {
                    if (this.Visible && PawnUtility.ShouldSendNotificationAbout(this.pawn))
                    {
                        Messages.Message("MessageMiscarriedPoorHealth".Translate(new object[]
                        {
                            this.pawn.LabelIndefinite()
                        }).CapitalizeFirst(), this.pawn, MessageTypeDefOf.NegativeHealthEvent);
                    }
                    this.Miscarry();
                    return;
                }
            }
            this.GestationProgress += 1f / (this.pawn.RaceProps.gestationPeriodDays * 60000f);
            if (this.GestationProgress >= 1f)
            {
                if (this.Visible && PawnUtility.ShouldSendNotificationAbout(this.pawn))
                {
                    Messages.Message("MessageGaveBirth".Translate(new object[]
                    {
                        this.pawn.LabelIndefinite()
                    }).CapitalizeFirst(), this.pawn, MessageTypeDefOf.PositiveEvent);
                }
                Hediff_AlienPregnant.DoBirthSpawn(this.pawn, this.father);
                this.pawn.health.RemoveHediff(this);
            }
        }

        // Token: 0x06004363 RID: 17251 RVA: 0x001E81F7 File Offset: 0x001E65F7
        private void Miscarry()
        {
            this.pawn.health.RemoveHediff(this);
        }

        // Token: 0x06004364 RID: 17252 RVA: 0x001E820C File Offset: 0x001E660C
        public static void DoBirthSpawn(Pawn mother, Pawn father)
        {
            int num = (mother.RaceProps.litterSizeCurve == null) ? 1 : Mathf.RoundToInt(Rand.ByCurve(mother.RaceProps.litterSizeCurve, 300));
            if (num < 1)
            {
                num = 1;
            }
            mother.needs.mood.thoughts.memories.TryGainMemory(AlienSexThoughtDefOf.NewColonyBabyMother);
            father.needs.mood.thoughts.memories.TryGainMemory(AlienSexThoughtDefOf.NewColonyBabyFather);
            mother.health.AddHediff(AlienSexHediffDefOf.AlienGaveBirth);
            mother.health.AddHediff(AlienSexHediffDefOf.AlienLactating);
            foreach (Pawn pawn2 in mother.Map.mapPawns.SpawnedPawnsInFaction(mother.Faction))
            {
                if (pawn2 != mother && pawn2 != father && pawn2.needs.mood != null && pawn2.needs.mood.thoughts != null)
                {
                    pawn2.needs.mood.thoughts.memories.TryGainMemory(AlienSexThoughtDefOf.NewColonyBaby, null);
                }
            }
            PawnKindDef babyRace = mother.kindDef;
            if (Rand.Value < 0.4f)
            {
                babyRace = father.kindDef;
            }
            else
            {
                babyRace = mother.kindDef;
            }
            PawnGenerationRequest request = new PawnGenerationRequest(babyRace, mother.Faction, PawnGenerationContext.NonPlayer, -1, false, true, false, false, true, false, 1f, false, true, true, false, false, false, false, null, null, null, null, null, null, null);
            Pawn pawn = null;
            for (int i = 0; i < num; i++)
            {
                pawn = PawnGenerator.GeneratePawn(request);
                pawn.health.AddHediff(AlienSexHediffDefOf.AlienInfant);
                pawn.skills.Learn(SkillDefOf.Animals, -100000);
                pawn.skills.Learn(SkillDefOf.Artistic, -100000);
                pawn.skills.Learn(SkillDefOf.Construction, -100000);
                pawn.skills.Learn(SkillDefOf.Cooking, -100000);
                pawn.skills.Learn(SkillDefOf.Crafting, -100000);
                pawn.skills.Learn(SkillDefOf.Growing, -100000);
                pawn.skills.Learn(SkillDefOf.Intellectual, -100000);
                pawn.skills.Learn(SkillDefOf.Medicine, -100000);
                pawn.skills.Learn(SkillDefOf.Melee, -100000);
                pawn.skills.Learn(SkillDefOf.Mining, -100000);
                pawn.skills.Learn(SkillDefOf.Shooting, -100000);
                pawn.skills.Learn(SkillDefOf.Social, -100000);
                pawn.story.childhood.SetTitle("Colony Baby");
                pawn.story.childhood.baseDesc = "Born within the colony, there is no memories or knowledge beyond the colony.";
                string text = "LetterNewAlienBaby".Translate(new object[]
                    {
                        mother,
                        father,
                        pawn.Name.ToStringFull,
                        pawn.KindLabel
                    });
                string label = "LetterLabelNewAlienBaby".Translate();
                Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.PositiveEvent, new TargetInfo(pawn.Position, pawn.Map, false), null);
                if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn, mother))
                {
                    if (pawn.playerSettings != null && mother.playerSettings != null)
                    {
                        pawn.playerSettings.AreaRestriction = mother.playerSettings.AreaRestriction;
                    }
                    if (pawn.RaceProps.IsFlesh)
                    {                        
                        pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, mother);
                        if (father != null)
                        {
                            pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, father);
                        }
                    }
                }
                else
                {
                    Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
                }
                TaleRecorder.RecordTale(TaleDefOf.GaveBirth, new object[]
                {
                    mother,
                    pawn
                });
            }
            if (mother.Spawned)
            {
                FilthMaker.MakeFilth(mother.Position, mother.Map, ThingDefOf.FilthAmnioticFluid, mother.LabelIndefinite(), 5);
                if (mother.caller != null)
                {
                    mother.caller.DoCall();
                }
                if (pawn.caller != null)
                {
                    pawn.caller.DoCall();
                }
            }
        }

        // Token: 0x06004365 RID: 17253 RVA: 0x001E83C2 File Offset: 0x001E67C2
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Pawn>(ref this.father, "father", false);
        }

        // Token: 0x06004366 RID: 17254 RVA: 0x001E83DC File Offset: 0x001E67DC
        public override string DebugString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.DebugString());
            stringBuilder.AppendLine("Gestation progress: " + this.GestationProgress.ToStringPercent());
            stringBuilder.AppendLine("Time left: " + ((int)((1f - this.GestationProgress) * this.pawn.RaceProps.gestationPeriodDays * 60000f)).ToStringTicksToPeriod(true, false, true));
            return stringBuilder.ToString();
        }

        // Token: 0x04002E8D RID: 11917
        public Pawn father;

        // Token: 0x04002E8E RID: 11918
        private const int MiscarryCheckInterval = 1000;

        // Token: 0x04002E8F RID: 11919
        private const float MTBMiscarryStarvingDays = 0.5f;

        // Token: 0x04002E90 RID: 11920
        private const float MTBMiscarryWoundedDays = 0.5f;
    }
}
