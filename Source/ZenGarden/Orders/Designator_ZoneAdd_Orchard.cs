using UnityEngine;
using RimWorld;
using Verse;
using System.Collections.Generic;

namespace ZenGarden
{

    public class Designator_ZoneAdd_Orchard : Designator_ZoneAdd
    {
        public List<ResearchProjectDef> researchPrerequisites;

        public override bool Visible
        {
            get
            {
                foreach (ThingDef current in ZenGardenMod.SecondaryPlants)
                {
                    List<ResearchProjectDef> sowResearchPrerequisites = current.plant.sowResearchPrerequisites;
                    if (sowResearchPrerequisites == null)
                    {
                        return true;
                    }
                    for (int i = 0; i < sowResearchPrerequisites.Count; i++)
                    {
                        if (!sowResearchPrerequisites[i].IsFinished)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        protected override string NewZoneLabel
        {
            get
            {
                return Static.LabelOrchardZone;
            }
        }


        public Designator_ZoneAdd_Orchard()
        {
            zoneTypeToPlace = typeof(Zone_Orchard);
            defaultLabel = Static.LabelOrchardZone;
            defaultDesc = "ZEN_DesignatorOrchardZoneDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("Cupro/UI/Designations/ZoneCreate_Orchard", true);
        }


        protected override Zone MakeNewZone()
        {
            return new Zone_Orchard(Find.CurrentMap.zoneManager);
        }


        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!base.CanDesignateCell(c).Accepted)
            {
                return false;
            }
            if (Map.fertilityGrid.FertilityAt(c) < ThingDefOf.Plant_Potato.plant.fertilityMin)
            {
                return false;
            }
            return true;
        }
    }
}
