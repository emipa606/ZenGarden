using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace ZenGarden;

public class WorkGiver_GrowerSowSecondary : WorkGiver_Scanner
{
    protected static ThingDef wantedPlantDef;

    public override PathEndMode PathEndMode => PathEndMode.ClosestTouch;


    private ThingDef CalculateWantedPlantDef(IntVec3 c, Map map)
    {
        var plantToGrowSettable = c.GetPlantToGrowSettable(map);
        if (plantToGrowSettable == null)
        {
            return null;
        }

        return plantToGrowSettable.GetPlantDefToGrow();
    }


    public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
    {
        var maxDanger = pawn.NormalMaxDanger();
        wantedPlantDef = null;
        var zonesList = pawn.Map.zoneManager.AllZones;
        foreach (var zone in zonesList)
        {
            if (zone is not Zone_Orchard orchardZone)
            {
                continue;
            }

            if (orchardZone.cells.Count == 0)
            {
                Log.ErrorOnce("Orchard zone has 0 cells: " + orchardZone, -563487);
                continue;
            }

            if (!ExtraRequirements(orchardZone, pawn))
            {
                continue;
            }

            if (orchardZone.ContainsStaticFire)
            {
                continue;
            }

            if (!pawn.CanReach(orchardZone.Cells[0], PathEndMode.OnCell, maxDanger))
            {
                continue;
            }

            foreach (var intVec3 in orchardZone.cells)
            {
                yield return intVec3;
            }

            wantedPlantDef = null;
        }

        wantedPlantDef = null;
    }


    private bool ExtraRequirements(IPlantToGrowSettable settable, Pawn pawn)
    {
        if (!settable.CanAcceptSowNow())
        {
            return false;
        }

        IntVec3 c;
        if (settable is Zone_Orchard orchardZone)
        {
            if (!orchardZone.allowSow)
            {
                return false;
            }

            c = orchardZone.Cells[0];
        }
        else
        {
            c = ((Thing)settable).Position;
        }

        wantedPlantDef = CalculateWantedPlantDef(c, pawn.Map);
        return wantedPlantDef != null;
    }

    // Not synced with CanGiveJob
    public override Job JobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
    {
        var map = pawn.Map;
        if (c.IsForbidden(pawn))
        {
            return null;
        }

        if (!PlantUtility.GrowthSeasonNow(c, pawn.Map))
        {
            return null;
        }

        if (wantedPlantDef == null)
        {
            wantedPlantDef = CalculateWantedPlantDef(c, pawn.Map);
            if (wantedPlantDef == null)
            {
                return null;
            }
        }

        var thingList = c.GetThingList(pawn.Map);
        var colonyPlant = false;
        var zone_Growing = c.GetZone(map) as Zone_Growing;
        foreach (var thing in thingList)
        {
            if (thing.def == wantedPlantDef)
            {
                return null;
            }

            if ((thing is Blueprint || thing is Frame) && thing.Faction == pawn.Faction)
            {
                colonyPlant = true;
            }
        }

        if (colonyPlant)
        {
            Thing edifice = c.GetEdifice(pawn.Map);
            if (edifice == null || edifice.def.fertility < 0f)
            {
                return null;
            }
        }

        if (wantedPlantDef.plant.interferesWithRoof && c.Roofed(pawn.Map))
        {
            return null;
        }

        var plant = c.GetPlant(pawn.Map);
        if (plant != null && plant.def.plant.blockAdjacentSow)
        {
            if (!pawn.CanReserve(plant, 1, -1, null, forced) || plant.IsForbidden(pawn))
            {
                return null;
            }

            if (zone_Growing is { allowCut: false })
            {
                return null;
            }

            if (!PlantUtility.PawnWillingToCutPlant_Job(plant, pawn))
            {
                return null;
            }

            return JobMaker.MakeJob(JobDefOf.CutPlant, plant);
        }

        var thing2 = PlantUtility.AdjacentSowBlocker(wantedPlantDef, c, pawn.Map);
        if (thing2 != null)
        {
            if (thing2 is not Plant plant2 ||
                !pawn.CanReserveAndReach(plant2, PathEndMode.Touch, Danger.Deadly, 1, -1, null, forced) ||
                plant2.IsForbidden(pawn))
            {
                return null;
            }

            var plantToGrowSettable = plant2.Position.GetPlantToGrowSettable(plant2.Map);
            if (plantToGrowSettable != null && plantToGrowSettable.GetPlantDefToGrow() == plant2.def)
            {
                return null;
            }

            if (thing2.Position.GetZone(map) is Zone_Growing { allowCut: false })
            {
                return null;
            }

            if (!PlantUtility.PawnWillingToCutPlant_Job(plant2, pawn))
            {
                return null;
            }

            return JobMaker.MakeJob(JobDefOf.CutPlant, plant2);
        }

        if (wantedPlantDef.plant.sowMinSkill > 0 && pawn.skills != null &&
            pawn.skills.GetSkill(SkillDefOf.Plants).Level < wantedPlantDef.plant.sowMinSkill)
        {
            return null;
        }

        foreach (var thing3 in thingList)
        {
            if (!thing3.def.BlocksPlanting())
            {
                continue;
            }

            if (!pawn.CanReserve(thing3, 1, -1, null, forced))
            {
                return null;
            }

            if (thing3.def.category == ThingCategory.Plant)
            {
                if (thing3.IsForbidden(pawn))
                {
                    return null;
                }

                if (zone_Growing is { allowCut: false })
                {
                    return null;
                }

                if (!PlantUtility.PawnWillingToCutPlant_Job(thing3, pawn))
                {
                    return null;
                }

                return JobMaker.MakeJob(JobDefOf.CutPlant, thing3);
            }

            if (thing3.def.EverHaulable)
            {
                return HaulAIUtility.HaulAsideJobFor(pawn, thing3);
            }

            return null;
        }

        if (!wantedPlantDef.CanEverPlantAt(c, pawn.Map) || !PlantUtility.GrowthSeasonNow(c, pawn.Map) ||
            !pawn.CanReserve(c))
        {
            return null;
        }

        return new Job(ZenDefOf.ZEN_PlantsSowSecondary, c)
        {
            plantDefToSow = wantedPlantDef
        };
    }
}