using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;

namespace Verse
{
    // Token: 0x02000C75 RID: 3189
    public class Hediff_AlienToddlerHungry : HediffWithComps
    {
        
        public override void Tick()
        {
            if (this.pawn.needs.food.CurLevelPercentage > 0.5f)
            {
                this.pawn.health.RemoveHediff(this);
            }
        }

    }
}
