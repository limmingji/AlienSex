using System;
using RimWorld;

namespace Verse.AI
{
        // Token: 0x020009DE RID: 2526
        public class MentalState_WanderToddler : MentalState
    {
        // Token: 0x060035A4 RID: 13732 RVA: 0x0018C97C File Offset: 0x0018AD7C
        public override RandomSocialMode SocialModeMax()
        {
            return RandomSocialMode.Normal;
        }
    }
}
