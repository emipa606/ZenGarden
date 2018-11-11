﻿using System.Collections.Generic;
using System.Linq;

using RimWorld;
using Verse;
using Verse.AI;

namespace ZenGarden
{

    public class WorkGiver_GrowerHarvestSecondary : WorkGiver_Scanner
    {

        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.Touch;
            }
        }


        public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
        {
            IEnumerable<Thing> list = pawn.Map.listerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.Plant)).Where(p => p is PlantWithSecondary);
            List<Zone> zonesList = pawn.Map.zoneManager.AllZones;

            foreach (Thing plant in list)
            {
                if (pawn.CanReach(plant, PathEndMode.OnCell, pawn.NormalMaxDanger(), false, TraverseMode.ByPawn))
                {
                    yield return plant.Position;
                }
            }

            for (int z = 0; z < zonesList.Count; z++)
            {
                if (zonesList[z] is Zone_Orchard orchardZone)
                {
                    if (orchardZone.cells.Count == 0)
                    {
                        Log.ErrorOnce("Orchard zone has 0 cells: " + orchardZone, -563487);
                    }
                    else if (!orchardZone.ContainsStaticFire)
                    {
                        if (pawn.CanReach(orchardZone.Cells[0], PathEndMode.OnCell, pawn.NormalMaxDanger(), false, TraverseMode.ByPawn))
                        {
                            for (int k = 0; k < orchardZone.cells.Count; k++)
                            {
                                yield return orchardZone.cells[k];
                            }
                        }
                    }
                }
            }
        }


        public override bool HasJobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
        {
            if(!c.InBounds(pawn.Map))
            {
                return false;
            }

            List<Thing> list = c.GetThingList(pawn.Map);
            PlantWithSecondary plant = null;
            for (int t = 0; t < list.Count; t++)
            {
                if (list[t] is PlantWithSecondary)
                {
                    plant = (PlantWithSecondary)list[t];
                    break;
                }
            }
            return plant != null && !plant.IsForbidden(pawn) && plant.Sec_HarvestableNow && pawn.CanReserve(plant) && HarvestableLocation(plant, c);
        }


        public override Job JobOnCell(Pawn pawn, IntVec3 c, bool forced = false)
        {
            Job job = new Job(ZenDefOf.ZEN_PlantsHarvestSecondary);
            Map map = pawn.Map;
            Room room = c.GetRoom(map, RegionType.Set_Passable);
            float num = 0f;
            for (int i = 0; i < 40; i++)
            {
                IntVec3 c2 = c + GenRadial.RadialPattern[i];
                if (c.GetRoom(map, RegionType.Set_Passable) == room)
                {
                    if (HasJobOnCell(pawn, c2))
                    {
                        Plant plant = c2.GetPlant(map);
                        num += 250;
                        if (num > 2400f)
                        {
                            break;
                        }
                        job.AddQueuedTarget(TargetIndex.A, plant);
                    }
                }
            }
            if (job.targetQueueA != null && job.targetQueueA.Count >= 3)
            {
                job.targetQueueA.SortBy((LocalTargetInfo targ) => targ.Cell.DistanceToSquared(pawn.Position));
            }
            return job;
        }





        private bool HasHarvestJobOnCell(Pawn pawn, IntVec3 c)
        {
            Plant plant = c.GetPlant(pawn.Map);
            if (!(plant is PlantWithSecondary))
            {
                return false;
            }
            PlantWithSecondary sec = (PlantWithSecondary)plant;
            return plant != null &&
                !plant.IsForbidden(pawn) &&
                sec.Sec_HarvestableNow &&
                plant.LifeStage == PlantLifeStage.Mature &&
                pawn.CanReserve(plant, 1, -1, null, false) &&
                HarvestableLocation(plant, c);
        }


        private bool HarvestableLocation(Plant plant, IntVec3 c)
        {
            if (c.GetZone(plant.Map) is Zone_Orchard)
            {
                return true;
            }
            return plant.Map.designationManager.DesignationOn(plant) != null && plant.Map.designationManager.DesignationOn(plant).def == ZenDefOf.ZEN_Designator_PlantsHarvestSecondary;
        }


        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t.def.category != ThingCategory.Plant)
            {
                return null;
            }

            if (!(t is PlantWithSecondary))
            {
                return null;
            }

            PlantWithSecondary plant = (PlantWithSecondary)t;
            if (!plant.Sec_HarvestableNow)
            {
                return null;
            }

            Job job = new Job(ZenDefOf.ZEN_PlantsHarvestSecondary);
            Map map = pawn.Map;
            Room room = t.Position.GetRoom(map, RegionType.Set_Passable);
            float num = 0f;
            for (int i = 0; i < 40; i++)
            {
                IntVec3 c2 = t.Position + GenRadial.RadialPattern[i];
                if (c2.GetRoom(map, RegionType.Set_Passable) == room)
                {
                    if (HasHarvestJobOnCell(pawn, c2))
                    {
                        num += 250f;
                        if (num > 2400f)
                        {
                            break;
                        }
                        job.AddQueuedTarget(TargetIndex.A, plant);
                    }
                }
            }
            if (job.targetQueueA != null && job.targetQueueA.Count >= 3)
            {
                job.targetQueueA.SortBy((LocalTargetInfo targ) => targ.Cell.DistanceToSquared(pawn.Position));
            }
            return job;
        }
    }
}
