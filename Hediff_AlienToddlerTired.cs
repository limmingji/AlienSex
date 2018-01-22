using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;

namespace Verse
{
    // Token: 0x02000C75 RID: 3189
    public class Hediff_AlienToddlerTired : HediffWithComps
    {

        public override void Tick()
        {
            if (this.pawn.needs.rest.CurLevel > 0.5f)
            {
                this.pawn.health.RemoveHediff(this);
            }
        }

    }
}
