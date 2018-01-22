using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.AI;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace AlienSexPatch
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.walkingproblem.aliensexpatch");

            // find the FillTab method of the class RimWorld.ITab_Pawn_Character
            MethodInfo targetmethod = AccessTools.Method(typeof(RimWorld.HealthCardUtility), "DrawOverviewTab");
            //MethodInfo targetmethod2 = AccessTools.Method(typeof(Verse.RaceProperties), "CanEverEat");

            // find the static method to call before (i.e. Prefix) the targetmethod
            HarmonyMethod postfixmethod = new HarmonyMethod(typeof(AlienSexPatch.HarmonyPatches).GetMethod("HealthCardUtilityDrawOverviewTab_Postfix"));
            //HarmonyMethod prefixmethod2 = new HarmonyMethod(typeof(AlienSexPatch.HarmonyPatches).GetMethod("RacePropertiesCanEverEat_Prefix"));

            // patch the targetmethod, by calling prefixmethod before it runs, with no postfixmethod (i.e. null)
            harmony.Patch(targetmethod, null, postfixmethod, null);
            //harmony.Patch(targetmethod2, prefixmethod2, null, null);
        }

        // This method is now always called right before RimWorld.ITab_Pawn_Character.FillTab.
        // So, before the ITab_Pawn_Character is instantiated, reset the height of the dialog window.
        // The class RimWorld.ITab_Pawn_Character is static so there is no this __instance.
        //public static Vector2 drawSizeHuman = new Vector2(0.5f, 0.5f);

        //public static void RacePropertiesCanEverEat_Prefix(ref bool __result, RaceProperties __instance, FoodTypeFlags food)
        //{
        //    if (__result)
        //    {
        //        return;
        //    }

        //    bool result = false;
        //    if (Hediff_HelplessInfant.limitedFood)
        //    {
        //        if (__instance.EatsFood && (__instance.foodType & food) == FoodTypeFlags.AnimalProduct)
        //        {
        //            result = true;
        //        }
        //    }
        //    __result = result;
        //}

        public static void HealthCardUtilityDrawOverviewTab_Postfix(Pawn pawn, Rect leftRect, ref float curY)
        {
            if (pawn.TryGetComp<AlienSexTracker>() != null)
            {
                GUI.color = Color.white;
                Text.Font = GameFont.Tiny;
                if (pawn.IsColonist && !pawn.Dead)
                {
                    bool wantAlienBaby = pawn.TryGetComp<AlienSexTracker>().wantAlienBaby;
                    Rect rect4 = new Rect(0f, curY, leftRect.width, 24f);
                    Widgets.CheckboxLabeled(rect4, "WillingToHaveBaby".Translate(), ref pawn.TryGetComp<AlienSexTracker>().wantAlienBaby, false);
                    //if (pawn.TryGetComp<AlienSexTracker>().wantAlienBaby && !wantAlienBaby)
                    //{
                    //    if (pawn.story.WorkTypeIsDisabled(WorkTypeDefOf.Doctor))
                    //    {
                    //        pawn.playerSettings.selfTend = false;
                    //        Messages.Message("MessageCannotSelfTendEver".Translate(new object[]
                    //        {
                    //            pawn.LabelShort
                    //        }), MessageTypeDefOf.RejectInput);
                    //    }
                    //    else if (pawn.workSettings.GetPriority(WorkTypeDefOf.Doctor) == 0)
                    //    {
                    //        Messages.Message("MessageSelfTendUnsatisfied".Translate(new object[]
                    //        {
                    //            pawn.LabelShort
                    //        }), MessageTypeDefOf.CautionInput);
                    //    }
                    //}
                    TooltipHandler.TipRegion(rect4, "WantAlienBabyTip".Translate(new object[]
                    {
                    Faction.OfPlayer.def.pawnsPlural,
                    0.7f.ToStringPercent()
                    }).CapitalizeFirst());
                    curY += 28f;
                }
            
            }

        }

    }
}
