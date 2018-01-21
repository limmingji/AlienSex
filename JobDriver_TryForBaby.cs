using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{

    [DefOf]
    public static class AlienSexHediffDefOf
    {
        // Token: 0x04001D46 RID: 7494
        public static HediffDef AlienPregnant;
        public static HediffDef AlienInfant;
        public static HediffDef AlienGaveBirth;
        
    }
        // Token: 0x02000071 RID: 113
        public class JobDriver_TryForBaby : JobDriver
    {
        [DefOf]
        public static class AlienSexThoughtDefOf
        {
            public static ThoughtDef TryForBaby;
        }

        [DefOf]
        public static class AlienSexJobDefOf
        {
            public static JobDef TryForBaby;
        }
        
            private Pawn Partner
        {
            get
            {
                return (Pawn)((Thing)this.job.GetTarget(this.PartnerInd));
            }
        }
        
        private Building_Bed Bed
        {
            get
            {
                return (Building_Bed)((Thing)this.job.GetTarget(this.BedInd));
            }
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksLeft, "ticksLeft", 0, false);
        }
        
        public override bool TryMakePreToilReservations()
        {
            return this.pawn.Reserve(this.Partner, this.job, 1, -1, null) && this.pawn.Reserve(this.Bed, this.job, this.Bed.SleepingSlotsCount, 0, null);
        }
        
        public override bool CanBeginNowWhileLyingDown()
        {
            return JobInBedUtility.InBedOrRestSpotNow(this.pawn, this.job.GetTarget(this.BedInd));
        }

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
            else if (Rand.Value < 0.5f && !female.health.hediffSet.HasHediff(AlienSexHediffDefOf.AlienPregnant, false))
            {
                Hediff_AlienPregnant hediff_Pregnant = (Hediff_AlienPregnant)HediffMaker.MakeHediff(AlienSexHediffDefOf.AlienPregnant, female, null);
                hediff_Pregnant.father = male;
                female.health.AddHediff(hediff_Pregnant, null, null);
            }
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(this.BedInd);
            this.FailOnDespawnedOrNull(this.PartnerInd);
            this.FailOn(() => !this.Partner.health.capacities.CanBeAwake);
            this.KeepLyingDown(this.BedInd);
            yield return Toils_Bed.ClaimBedIfNonMedical(this.BedInd, TargetIndex.None);
            yield return Toils_Bed.GotoBed(this.BedInd);
            yield return new Toil
            {
                initAction = delegate
                {
                    if (this.Partner.CurJob == null || this.Partner.CurJob.def != AlienSexJobDefOf.TryForBaby)
					{
                        Job newJob = new Job(AlienSexJobDefOf.TryForBaby, this.pawn, this.Bed);
                        this.Partner.jobs.StartJob(newJob, JobCondition.InterruptForced, null, false, true, null, null, false);
                        this.ticksLeft = (int)(2500f * Mathf.Clamp(Rand.Range(0.1f, 1.1f), 0.1f, 2f));
                    }
					else
					{
                        this.ticksLeft = 9999999;
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            Toil doLovin = Toils_LayDown.LayDown(this.BedInd, true, false, false, false);
            doLovin.FailOn(() => this.Partner.CurJob == null || this.Partner.CurJob.def != AlienSexJobDefOf.TryForBaby);
            doLovin.AddPreTickAction(delegate
            {
            this.ticksLeft--;
            if (this.ticksLeft <= 0)

                {
                this.ReadyForNextToil();
            }
				else if (this.pawn.IsHashIntervalTick(100))
				{
                MoteMaker.ThrowMetaIcon(this.pawn.Position, this.pawn.Map, ThingDefOf.Mote_Heart);
            }
        });
			doLovin.AddFinishAction(delegate
			{
                if (this.pawn.gender == Gender.Female && this.Partner.gender == Gender.Male)
                {
                    Mated(this.Partner, this.pawn);
                }
                if (this.pawn.gender == Gender.Male && this.Partner.gender == Gender.Female)
                {
                    Mated(this.pawn, this.Partner);
                }
                Thought_Memory newThought = (Thought_Memory)ThoughtMaker.MakeThought(AlienSexThoughtDefOf.TryForBaby);
				this.pawn.needs.mood.thoughts.memories.TryGainMemory(newThought, this.Partner);
				this.pawn.mindState.canLovinTick = Find.TickManager.TicksGame + this.GenerateRandomMinTicksToNextLovin(this.pawn);
                

        });
			doLovin.socialMode = RandomSocialMode.Off;
			yield return doLovin;
			yield break;
		}

    // Token: 0x06000300 RID: 768 RVA: 0x0001C3A8 File Offset: 0x0001A7A8
    private int GenerateRandomMinTicksToNextLovin(Pawn pawn)
    {
        if (DebugSettings.alwaysDoLovin)
        {
            return 100;
        }
        float num = JobDriver_TryForBaby.LovinIntervalHoursFromAgeCurve.Evaluate(pawn.ageTracker.AgeBiologicalYearsFloat);
        num = Rand.Gaussian(num, 0.3f);
        if (num < 0.5f)
        {
            num = 0.5f;
        }
        return (int)(num * 2500f);
    }

    // Token: 0x04000214 RID: 532
    private int ticksLeft;

    // Token: 0x04000215 RID: 533
    private TargetIndex PartnerInd = TargetIndex.A;

    // Token: 0x04000216 RID: 534
    private TargetIndex BedInd = TargetIndex.B;

    // Token: 0x04000217 RID: 535
    private const int TicksBetweenHeartMotes = 100;

    // Token: 0x04000218 RID: 536
    private static readonly SimpleCurve LovinIntervalHoursFromAgeCurve = new SimpleCurve
        {
            {
                new CurvePoint(16f, 1.5f),
                true
            },
            {
                new CurvePoint(22f, 1.5f),
                true
            },
            {
                new CurvePoint(30f, 4f),
                true
            },
            {
                new CurvePoint(50f, 12f),
                true
            },
            {
                new CurvePoint(75f, 36f),
                true
            }
        };
}
}
