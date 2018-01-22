using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;

namespace Verse
{
    // Token: 0x02000C75 RID: 3189
    public class Hediff_HelplessInfant : HediffWithComps
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

        //public static bool limitedFood = false;
        

        public override void Tick()
        {
            this.pawn.workSettings.DisableAll();
            this.pawn.RaceProps.baseHungerRate = 2f;
            //this.pawn.RaceProps.foodType = FoodTypeFlags.AnimalProduct;
            //limitedFood = true;
            float helplessdays = pawn.kindDef.race.GetStatValueAbstract(AlienSexStatDefOf.HelplessInfantDays);
            this.ageTicks++;
            this.InfancyProgress += 1f / (helplessdays * 60000f);
            if (this.InfancyProgress >= 1f)
            {
                //limitedFood = false;
                this.pawn.health.AddHediff(AlienSexHediffDefOf.AlienToddler);
                this.pawn.health.RemoveHediff(this);
            }
        }

        public override string DebugString()
        {
            float helplessdays = pawn.kindDef.race.GetStatValueAbstract(AlienSexStatDefOf.HelplessInfantDays);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.DebugString());
            stringBuilder.AppendLine("Infancy progress: " + this.InfancyProgress.ToStringPercent());
            stringBuilder.AppendLine("Time left: " + ((int)((1f - this.InfancyProgress) * helplessdays * 60000f)).ToStringTicksToPeriod(true, false, true));
            return stringBuilder.ToString();
        }

    }
}
