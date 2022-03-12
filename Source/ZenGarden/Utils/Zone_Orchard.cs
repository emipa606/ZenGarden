using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace ZenGarden;

public class Zone_Orchard : Zone, IPlantToGrowSettable
{
    public bool allowSow = true;

    private ThingDef plantDefToGrow = ZenDefOf.ZEN_PlantTreeCherry;


    public Zone_Orchard()
    {
    }


    public Zone_Orchard(ZoneManager zoneManager) : base(Static.LabelOrchardZone, zoneManager)
    {
    }

    public override bool IsMultiselectable => true;

    protected override Color NextZoneColor => ZoneColorUtility.NextGrowingZoneColor();

    public bool CanAcceptSowNow()
    {
        return allowSow;
    }

    IEnumerable<IntVec3> IPlantToGrowSettable.Cells => Cells;


    public ThingDef GetPlantDefToGrow()
    {
        return plantDefToGrow;
    }

    public void SetPlantDefToGrow(ThingDef plantDef)
    {
        if (plantDef.thingClass == typeof(PlantWithSecondary))
        {
            plantDefToGrow = plantDef;
        }
    }

    public override void AddCell(IntVec3 c)
    {
        base.AddCell(c);
        foreach (var t in Map.thingGrid.ThingsListAt(c))
        {
            Designator_PlantsHarvestWood.PossiblyWarnPlayerImportantPlantDesignateCut(t);
        }
    }

    public override IEnumerable<Gizmo> GetZoneAddGizmos()
    {
        yield return DesignatorUtility.FindAllowedDesignator<Designator_ZoneAdd_Orchard_Expand>();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Defs.Look(ref plantDefToGrow, "plantDefToGrow");
        Scribe_Values.Look(ref allowSow, "allowSow", true);
    }


    public override IEnumerable<Gizmo> GetGizmos()
    {
        foreach (var g in base.GetGizmos())
        {
            yield return g;
        }

        yield return SecondaryPlantToGrowSettableUtility.SetPlantCommand(this);
        yield return new Command_Toggle
        {
            defaultLabel = "CommandAllowSow".Translate(),
            defaultDesc = "CommandAllowSowDesc".Translate(),
            hotKey = KeyBindingDefOf.Command_ItemForbid,
            icon = TexCommand.ForbidOn,
            isActive = () => allowSow,
            toggleAction = delegate { allowSow = !allowSow; }
        };
    }


    public override string GetInspectString()
    {
        var text = string.Empty;
        if (Cells.NullOrEmpty())
        {
            return text;
        }

        var c = Cells.First();
        if (c.UsesOutdoorTemperature(Map))
        {
            var text2 = text;
            text = string.Concat(text2, "OutdoorGrowingPeriod".Translate(), ": ",
                Zone_Growing.GrowingQuadrumsDescription(Map.Tile), "\n");
        }

        if (PlantUtility.GrowthSeasonNow(c, Map))
        {
            text += "GrowSeasonHereNow".Translate();
        }
        else
        {
            text += "CannotGrowBadSeasonTemperature".Translate();
        }

        return text;
    }


    public static string GrowingQuadrumsDescription(int tile)
    {
        var list = GenTemperature.TwelfthsInAverageTemperatureRange(tile, 10f, 42f);
        if (list.NullOrEmpty())
        {
            return "NoGrowingPeriod".Translate();
        }

        if (list.Count == 12)
        {
            return "GrowYearRound".Translate();
        }

        return "PeriodDays".Translate(list.Count * 5
        ) + " (" + QuadrumUtility.QuadrumsRangeLabel(list) + ")";
    }
}