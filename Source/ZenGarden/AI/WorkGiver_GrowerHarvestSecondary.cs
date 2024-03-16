using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace ZenGarden;

public class WorkGiver_GrowerHarvestSecondary : WorkGiver_Grower
{
    public override PathEndMode PathEndMode => PathEndMode.Touch;


    public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
    {
        var list = pawn.Map.listerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.Plant))
            .Where(p => p is PlantWithSecondary);
        var zonesList = pawn.Map.zoneManager.AllZones;

        foreach (var plant in list)
        {
            if (pawn.CanReach(plant, PathEndMode.OnCell, pawn.NormalMaxDanger()))
            {
                yield return plant.Position;
            }
        }

        foreach (var zone in zonesList)
        {
            if (zone is not Zone_Orchard orchardZone)
            {
                continue;
            }

            if (orchardZone.cells.Count == 0)
            {
                Log.ErrorOnce($"Orchard zone has 0 cells: {orchardZone}", -563487);
                continue;
            }

            if (orchardZone.ContainsStaticFire ||
                !pawn.CanReach(orchardZone.Cells[0], PathEndMode.OnCell, pawn.NormalMaxDanger()))
            {
                continue;
            }

            foreach (var intVec3 in orchardZone.cells)
            {
                yield return intVec3;
            }
        }
    }


    public override bool HasJobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
    {
        var plant = c.GetPlant(pawn.Map);
        if (plant == null)
        {
            return false;
        }

        if (plant.IsForbidden(pawn))
        {
            return false;
        }

        if (plant is not PlantWithSecondary plantWithSecondary)
        {
            return false;
        }

        if (!plantWithSecondary.Sec_HarvestableNow)
        {
            return false;
        }

        if (plantWithSecondary.LifeStage != PlantLifeStage.Mature)
        {
            return false;
        }

        return pawn.CanReserve(plantWithSecondary, 1, -1, null, forced) && HarvestableLocation(plantWithSecondary, c);
    }

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        return pawn.GetLord() != null || base.ShouldSkip(pawn, forced);
    }

    public override Job JobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
    {
        var job = new Job(ZenDefOf.ZEN_PlantsHarvestSecondary);
        var map = pawn.Map;
        var room = c.GetRoom(map);
        var num = 0f;
        Plant plant;
        if (c.GetRoom(map) == room && HasJobOnCell(pawn, c))
        {
            plant = c.GetPlant(map);
            job.AddQueuedTarget(TargetIndex.A, plant);
        }

        for (var i = 0; i < 40; i++)
        {
            var c2 = c + GenRadial.RadialPattern[i];

            if (!HasJobOnCell(pawn, c2))
            {
                continue;
            }

            num += 250;
            if (num > 2400f)
            {
                break;
            }

            plant = c2.GetPlant(map);

            job.AddQueuedTarget(TargetIndex.A, plant);
        }

        if (job.targetQueueA is { Count: >= 3 })
        {
            job.targetQueueA.SortBy(targ => targ.Cell.DistanceToSquared(pawn.Position));
        }

        return job;
    }


    private bool HarvestableLocation(Plant plant, IntVec3 c)
    {
        if (c.GetZone(plant.Map) is Zone_Orchard)
        {
            return true;
        }

        return plant.Map.designationManager.DesignationOn(plant)?.def == ZenDefOf.ZEN_Designator_PlantsHarvestSecondary;
    }
}