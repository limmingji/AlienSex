using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
    // Token: 0x020001D7 RID: 471
    public class ThinkNode_ConditionalPawnMinimumAge : ThinkNode_Conditional
    {
        public override ThinkNode DeepCopy(bool resolve = true)
        {
            ThinkNode_ConditionalPawnMinimumAge thinkNode_ConditionalPawnMinimumAge = (ThinkNode_ConditionalPawnMinimumAge)base.DeepCopy(resolve);
            thinkNode_ConditionalPawnMinimumAge.maxAge = this.maxAge;
            thinkNode_ConditionalPawnMinimumAge.minAge = this.minAge;
            return thinkNode_ConditionalPawnMinimumAge;
        }
        // Token: 0x06000914 RID: 2324 RVA: 0x0004768B File Offset: 0x00045A8B
        protected override bool Satisfied(Pawn pawn)
        {
            float age = pawn.ageTracker.AgeBiologicalYearsFloat;
            return age >= this.minAge && age <= this.maxAge;
        }

        // Token: 0x040003C8 RID: 968
        float minAge;
        float maxAge;

    }
}
