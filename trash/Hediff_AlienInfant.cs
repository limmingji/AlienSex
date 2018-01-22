using System;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;

namespace Verse
{
    // Token: 0x02000C7A RID: 3194
    public class Hediff_AlienInfant : HediffWithComps
    {
        public Thought_Memory GiveObservedThought()
        {
            Thought_MemoryObservation thought_MemoryObservation;
            thought_MemoryObservation = (Thought_MemoryObservation)ThoughtMaker.MakeThought(AlienSexThoughtDefOf.SeeAlienBaby);
            thought_MemoryObservation.Target = this.pawn;
            return thought_MemoryObservation;
            
        }
    }
}