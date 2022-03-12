using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ZenGarden;

public class Designator_ZoneAdd_Orchard : Designator_ZoneAdd
{
    public List<ResearchProjectDef> researchPrerequisites;


    public Designator_ZoneAdd_Orchard()
    {
        zoneTypeToPlace = typeof(Zone_Orchard);
        defaultLabel = Static.LabelOrchardZone;
        defaultDesc = "ZEN_DesignatorOrchardZoneDesc".Translate();
        icon = ContentFinder<Texture2D>.Get("Cupro/UI/Designations/ZoneCreate_Orchard");
    }

    public override bool Visible
    {
        get
        {
            foreach (var current in ZenGardenMod.SecondaryPlants)
            {
                var sowResearchPrerequisites = current.plant.sowResearchPrerequisites;
                if (sowResearchPrerequisites == null)
                {
                    return true;
                }

                foreach (var researchProjectDef in sowResearchPrerequisites)
                {
                    if (!researchProjectDef.IsFinished)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    protected override string NewZoneLabel => Static.LabelOrchardZone;


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