using HarmonyLib;
using RimWorld;
using Verse;

namespace ZenGarden;

[HarmonyPatch(typeof(WorkGiver_GrowerHarvest), "HasJobOnCell", typeof(Pawn), typeof(IntVec3))]
public static class WorkGiver_GrowerHarvest_HasJobOnCell
{
    public static void Postfix(Pawn pawn, IntVec3 c, ref bool __result)
    {
        var plant = c.GetPlant(pawn.Map);
        // If this plant has a secondary resource and is in an orchard zone, don't chop it down unless it's designated
        if (plant is not PlantWithSecondary || pawn.Map.zoneManager.ZoneAt(c) is not Zone_Orchard)
        {
            return;
        }

        if (pawn.Map.designationManager.DesignationAt(c, DesignationDefOf.CutPlant) == null)
        {
            __result = false;
        }
    }
}