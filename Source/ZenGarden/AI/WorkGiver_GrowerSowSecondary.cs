using System.Collections.Generic;

using RimWorld;
using Verse;
using Verse.AI;

namespace ZenGarden {

	public class WorkGiver_GrowerSowSecondary : WorkGiver_Scanner {

		protected static ThingDef wantedPlantDef;

		public override PathEndMode PathEndMode {
			get {
				return PathEndMode.ClosestTouch;
			}
		}


		private ThingDef CalculateWantedPlantDef(IntVec3 c, Map map) {
			IPlantToGrowSettable plantToGrowSettable = c.GetPlantToGrowSettable(map);
			if (plantToGrowSettable == null) {
				return null;
			}
			return plantToGrowSettable.GetPlantDefToGrow();
		}


		public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn) {
			Danger maxDanger = pawn.NormalMaxDanger();
			wantedPlantDef = null;
			List<Zone> zonesList = pawn.Map.zoneManager.AllZones;
			for (int z = 0; z < zonesList.Count; z++) {
				if (zonesList[z] is Zone_Orchard orchardZone) {
					if (orchardZone.cells.Count == 0) {
						Log.ErrorOnce("Orchard zone has 0 cells: " + orchardZone, -563487);
					}
					else if (ExtraRequirements(orchardZone, pawn)) {
						if (!orchardZone.ContainsStaticFire) {
							if (pawn.CanReach(orchardZone.Cells[0], PathEndMode.OnCell, maxDanger)) {
								for (int k = 0; k < orchardZone.cells.Count; k++) {
									yield return orchardZone.cells[k];
								}
								wantedPlantDef = null;
							}
						} 
					}
				}
			}
			wantedPlantDef = null;
		}


		private bool ExtraRequirements(IPlantToGrowSettable settable, Pawn pawn) {
			if (!settable.CanAcceptSowNow()) {
				return false;
			}
			Zone_Orchard orchardZone = settable as Zone_Orchard;
			IntVec3 c;
			if (orchardZone != null) {
				if (!orchardZone.allowSow) {
					return false;
				}
				c = orchardZone.Cells[0];
			}
			else {
				c = ((Thing)settable).Position;
			}
			wantedPlantDef = CalculateWantedPlantDef(c, pawn.Map);
			return wantedPlantDef != null;
		}

		// Not synced with CanGiveJob
		public override Job JobOnCell(Pawn pawn, IntVec3 c, bool forced = false) {
			Map map = pawn.Map;
			if (c.IsForbidden(pawn)) {
				return null;
			}
			if (!PlantUtility.GrowthSeasonNow(c, pawn.Map)) {
				return null;
			}
			if (wantedPlantDef == null) {
				wantedPlantDef = CalculateWantedPlantDef(c, pawn.Map);
				if (wantedPlantDef == null) {
					return null;
				}
			}
			List<Thing> thingList = c.GetThingList(pawn.Map);
			bool flag = false;
			Zone_Growing zone_Growing = c.GetZone(map) as Zone_Growing;
			for (int i = 0; i < thingList.Count; i++) {
				Thing thing = thingList[i];
				if (thing.def == wantedPlantDef) {
					return null;
				}
				if ((thing is Blueprint || thing is Frame) && thing.Faction == pawn.Faction)
				{
					flag = true;
				}
			}
			
			if (flag)
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

			Plant plant = c.GetPlant(pawn.Map);
			if (plant != null && plant.def.plant.blockAdjacentSow) {
				if (!pawn.CanReserve(plant, 1, -1, null, forced) || plant.IsForbidden(pawn))
				{
					return null;
				}
				if (zone_Growing != null && !zone_Growing.allowCut)
				{
					return null;
				}
				if (!PlantUtility.PawnWillingToCutPlant_Job(plant, pawn))
				{
					return null;
				}
				return JobMaker.MakeJob(JobDefOf.CutPlant, plant);
			}
			else {
				Thing thing2 = PlantUtility.AdjacentSowBlocker(wantedPlantDef, c, pawn.Map);
				if (thing2 != null) {
					Plant plant2 = thing2 as Plant;
					if (plant2 != null && pawn.CanReserveAndReach(plant2, PathEndMode.Touch, Danger.Deadly, 1, -1, null, forced) && !plant2.IsForbidden(pawn))
					{
						IPlantToGrowSettable plantToGrowSettable = plant2.Position.GetPlantToGrowSettable(plant2.Map);
						if (plantToGrowSettable == null || plantToGrowSettable.GetPlantDefToGrow() != plant2.def)
						{
							Zone_Growing zone_Growing2 = thing2.Position.GetZone(map) as Zone_Growing;
							if (zone_Growing2 != null && !zone_Growing2.allowCut)
							{
								return null;
							}
							if (!PlantUtility.PawnWillingToCutPlant_Job(plant2, pawn))
							{
								return null;
							}
							return JobMaker.MakeJob(JobDefOf.CutPlant, plant2);
						}
					}
					return null;
				}
				if (wantedPlantDef.plant.sowMinSkill > 0 && pawn.skills != null && pawn.skills.GetSkill(SkillDefOf.Plants).Level < wantedPlantDef.plant.sowMinSkill) {
					return null;
				}
				for (int j = 0; j < thingList.Count; j++)
				{ 
					Thing thing3 = thingList[j];
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
						if (!thing3.IsForbidden(pawn))
						{
							if (zone_Growing != null && !zone_Growing.allowCut)
							{
								return null;
							}
							if (!PlantUtility.PawnWillingToCutPlant_Job(thing3, pawn))
							{
								return null;
							}
							return JobMaker.MakeJob(JobDefOf.CutPlant, thing3);
						}
						return null;
					}
					if (thing3.def.EverHaulable)
					{
						return HaulAIUtility.HaulAsideJobFor(pawn, thing3);
					}
					return null;

				}
				if (!wantedPlantDef.CanEverPlantAt(c, pawn.Map) || !PlantUtility.GrowthSeasonNow(c, pawn.Map) || !pawn.CanReserve(c, 1, -1, null, false)) {
					return null;
				}
				return new Job(ZenDefOf.ZEN_PlantsSowSecondary, c) {
					plantDefToSow = wantedPlantDef
				};
			}
		}
	}
}
