using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;

namespace Verse
{
    // Token: 0x02000C75 RID: 3189
    public class Hediff_AlienLactating : HediffWithComps
    {

        public float InfancyProgress
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

        public float AlienLactateNow = 0f;

        public override void Tick()
        {
            ThingDef milk = ThingDef.Named("Milk");
            float MilkAmount = pawn.kindDef.race.GetStatValueAbstract(AlienSexStatDefOf.AmountofAlienMilk);
            float LactateFreq = pawn.kindDef.race.GetStatValueAbstract(AlienSexStatDefOf.LactatingFrequency);
            int milkAmount = Mathf.RoundToInt(MilkAmount);
            
            AlienLactateNow += 1f / (LactateFreq * 60000f);
            if (AlienLactateNow >= 1f)
            {
                //for (int i = 0; i < MilkAmount; i++)
                //{
                Thing milk2 = GenSpawn.Spawn(milk, this.pawn.Position, this.pawn.Map);
                milk2.stackCount = milkAmount;

                //}
                AlienLactateNow = 0f;

            }          
            float helplessdays = pawn.kindDef.race.GetStatValueAbstract(AlienSexStatDefOf.HelplessInfantDays);
            this.ageTicks++;
            this.InfancyProgress += 1f / (helplessdays * 60000f);
            if (this.InfancyProgress >= 1f)
            {
                this.pawn.health.RemoveHediff(this);
            }
        }

        public override string DebugString()
        {
            float helplessdays = pawn.kindDef.race.GetStatValueAbstract(AlienSexStatDefOf.HelplessInfantDays);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.DebugString());
            stringBuilder.AppendLine("Time to lactate again: " + this.AlienLactateNow.ToStringPercent());
            stringBuilder.AppendLine("Time left to end of lactation: " + ((int)((1f - this.InfancyProgress) * helplessdays * 60000f)).ToStringTicksToPeriod(true, false, true));
            return stringBuilder.ToString();
        }

    }
}
