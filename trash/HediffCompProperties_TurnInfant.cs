using System;

namespace Verse
{
    // Token: 0x02000C50 RID: 3152
    public class HediffCompProperties_TurnInfant : HediffCompProperties
    {
        // Token: 0x06004294 RID: 17044 RVA: 0x001E5010 File Offset: 0x001E3410
        public HediffCompProperties_TurnInfant()
        {
            this.compClass = typeof(HediffComp_Disappears);
        }

        public PawnKindDef infantRace = default(PawnKindDef);


        // Token: 0x04002E3A RID: 11834
        public IntRange disappearsAfterTicks = default(IntRange);
    }
}
