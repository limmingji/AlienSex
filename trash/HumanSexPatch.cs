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

namespace HumanSexPatch
{
    [StaticConstructorOnStartup]
    static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("rimworld.walkingproblem.humansexpatch");

            // find the FillTab method of the class RimWorld.ITab_Pawn_Character
            MethodInfo targetmethod = AccessTools.Method(typeof(Verse.PawnRenderer), "PawnRenderer");

            // find the static method to call before (i.e. Prefix) the targetmethod
            HarmonyMethod prefixmethod = new HarmonyMethod(typeof(HumanSexPatch.HarmonyPatches).GetMethod("PawnRendererPawnRenderer_Prefix"));

            // patch the targetmethod, by calling prefixmethod before it runs, with no postfixmethod (i.e. null)
            harmony.Patch(targetmethod, null, prefixmethod, null);
        }

        // This method is now always called right before RimWorld.ITab_Pawn_Character.FillTab.
        // So, before the ITab_Pawn_Character is instantiated, reset the height of the dialog window.
        // The class RimWorld.ITab_Pawn_Character is static so there is no this __instance.
        public static Vector2 drawSizeHuman = new Vector2(0.5f,0.5f);

        

        public PawnRendererPawnRenderer_Prefix(PawnRenderer.PawnRenderer __instance, Pawn pawn)
        {
            __instance.pawn.Drawer.renderer.graphics.nakedGraphic.drawSize = drawSizeHuman }
    }
}
