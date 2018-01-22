using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;

namespace Verse
{
    // Token: 0x02000C75 RID: 3189
    public class Hediff_AlienToddler : HediffWithComps
    {

        public float ToddlerProgress
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
        
        public override void Tick()
        {
            this.pawn.workSettings.DisableAll();
            //this.pawn.mindState.mentalStateHandler.TryStartMentalState(AlienSexMentalStateDefOf.WanderToddler);
            this.pawn.RaceProps.baseHungerRate = 1.5f;
            if (this.pawn.needs.food.CurLevelPercentage < 0.5f)
            {
                this.pawn.health.AddHediff(AlienSexHediffDefOf.AlienToddlerHungry);
            }
            if (this.pawn.needs.rest.CurLevel < 0.5f)
            {
                this.pawn.health.AddHediff(AlienSexHediffDefOf.AlienToddlerTired);
            }

            float troublesomedays = pawn.kindDef.race.GetStatValueAbstract(AlienSexStatDefOf.AlienToddlerDays);
            this.ageTicks++;
            this.ToddlerProgress += 1f / (troublesomedays * 60000f);
            if (this.ToddlerProgress >= 1f)
            {
                this.pawn.health.RemoveHediff(this);
            }
        }

        public override string DebugString()
        {
            float troublesomedays = pawn.kindDef.race.GetStatValueAbstract(AlienSexStatDefOf.AlienToddlerDays);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.DebugString());
            stringBuilder.AppendLine("Troublesome toddlerhood progress: " + this.ToddlerProgress.ToStringPercent());
            stringBuilder.AppendLine("Time left: " + ((int)((1f - this.ToddlerProgress) * troublesomedays * 60000f)).ToStringTicksToPeriod(true, false, true));
            return stringBuilder.ToString();
        }

    }
}
