using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;

namespace Verse
{
    // Token: 0x02000C9F RID: 3231
    public class AlienSexTracker : ThingComp
    {
        public bool wantAlienBaby;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref this.wantAlienBaby, "wantAlienBaby", true);
        }
    }
}