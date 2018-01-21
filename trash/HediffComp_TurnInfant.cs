using System;
using Verse;
using RimWorld;
using RimWorld.Planet;
using Verse.AI;

namespace Verse
{
    // Token: 0x02000C51 RID: 3153
    public class HediffComp_TurnInfant : HediffComp
    {
        // Token: 0x17000A5E RID: 2654
        // (get) Token: 0x06004296 RID: 17046 RVA: 0x001E504A File Offset: 0x001E344A
        public HediffCompProperties_TurnInfant Props
        {
            get
            {
                return (HediffCompProperties_TurnInfant)this.props;
            }
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            if (!base.Pawn.Dead)
            {
                float bioyears = base.Pawn.ageTracker.AgeBiologicalYears;
                float chronoyears = base.Pawn.ageTracker.AgeChronologicalYears;
                Name name = base.Pawn.Name;
                Pawn_PlayerSettings playersetting = base.Pawn.playerSettings;
                Pawn_TrainingTracker training = base.Pawn.training;
                Pawn_HealthTracker health = base.Pawn.health;
                Pawn_RecordsTracker records = base.Pawn.records;
                Pawn_RelationsTracker relation = base.Pawn.relations;
                Pawn_SkillTracker skill = base.Pawn.skills;
                Pawn_NeedsTracker needs = base.Pawn.needs;
                Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(turnInfant, base.Pawn.Faction, PawnGenerationContext.All, -1, false, true, false, false, true, false, 1f, false, true, true, false, false, false, false, null, null, null, null, null, null, null));
                GenSpawn.Spawn(pawn, base.Pawn.Position, base.Pawn.Map, Rot4.Random);
                pawn.Name = name;
                pawn.relations = relation;
                pawn.training = training;
                pawn.records = records;
                pawn.skills = skill;
                pawn.playerSettings = playersetting;
                pawn.needs = needs;
                pawn.health.AddHediff(HediffMaker.MakeHediff(HediffDefOf.BadBack, pawn, null));
                base.Pawn.Destroy(DestroyMode.Vanish);
            }
        }


        private PawnKindDef turnInfant;
    }
}
