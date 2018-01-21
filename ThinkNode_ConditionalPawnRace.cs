using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020001D7 RID: 471
    public class ThinkNode_ConditionalPawnRace : ThinkNode_Conditional
    {
        // Token: 0x06000913 RID: 2323 RVA: 0x00047664 File Offset: 0x00045A64
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalPawnRace thinkNode_ConditionalPawnRace = (ThinkNode_ConditionalPawnRace)base.DeepCopy(resolve);
            thinkNode_ConditionalPawnRace.race = this.race;
            return thinkNode_ConditionalPawnRace;
        }

        // Token: 0x06000914 RID: 2324 RVA: 0x0004768B File Offset: 0x00045A8B
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.kindDef.race == this.race;
        }

        // Token: 0x040003C8 RID: 968
        ThingDef race;

    }
}
