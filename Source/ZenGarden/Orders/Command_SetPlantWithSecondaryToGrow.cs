using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace ZenGarden;

[StaticConstructorOnStartup]
public class Command_SetPlantWithSecondaryToGrow : Command
{
    private static readonly Texture2D SetPlantToGrowTex = ContentFinder<Texture2D>.Get("UI/Commands/SetPlantToGrow");
    public IPlantToGrowSettable settable;

    private List<IPlantToGrowSettable> settables;


    public Command_SetPlantWithSecondaryToGrow()
    {
        ThingDef thingDef = null;
        var secondaryThingSet = false;
        foreach (var current in Find.Selector.SelectedObjects)
        {
            if (current is not IPlantToGrowSettable plantToGrowSettable)
            {
                continue;
            }

            if (thingDef != null && thingDef != plantToGrowSettable.GetPlantDefToGrow())
            {
                secondaryThingSet = true;
                break;
            }

            thingDef = plantToGrowSettable.GetPlantDefToGrow();
        }

        if (secondaryThingSet)
        {
            icon = SetPlantToGrowTex;
            defaultLabel = "CommandSelectPlantToGrowMulti".Translate();
        }
        else
        {
            icon = thingDef?.uiIcon;
            defaultLabel = "CommandSelectPlantToGrow".Translate(thingDef?.label
            );
        }
    }


    public override void ProcessInput(Event ev)
    {
        base.ProcessInput(ev);
        var list = new List<FloatMenuOption>();
        if (settables == null)
        {
            settables = new List<IPlantToGrowSettable>();
        }

        if (!settables.Contains(settable))
        {
            settables.Add(settable);
        }

        foreach (var current in ZenGardenMod.SecondaryPlants)
        {
            if (!IsValidPlant(current) || !IsPlantAvailable(current))
            {
                continue;
            }

            var localPlantDef = current;
            string text = current.LabelCap;
            if (current.plant.sowMinSkill > 0)
            {
                var text2 = text;
                text = string.Concat(text2, " (", "MinSkill".Translate(), ": ", current.plant.sowMinSkill, ")");
            }

            bool ExtraPartOnGui(Rect rect)
            {
                return Widgets.InfoCardButton(rect.x + 5f, rect.y + ((rect.height - 24f) / 2f), localPlantDef);
            }

            list.Add(new FloatMenuOption(text, delegate
            {
                var unused = localPlantDef.defName;
                foreach (var growSettable in settables)
                {
                    growSettable.SetPlantDefToGrow(localPlantDef);
                }

                WarnAsAppropriate(localPlantDef);
            }, MenuOptionPriority.Default, null, null, 29f, ExtraPartOnGui));
        }

        Find.WindowStack.Add(new FloatMenu(list));
    }


    private static bool IsValidPlant(ThingDef plant)
    {
        if (plant.GetModExtension<SecondaryResource>().forbiddenGrowBiomes.NullOrEmpty() || !plant
                .GetModExtension<SecondaryResource>().forbiddenGrowBiomes.Contains(Find.CurrentMap.Biome))
        {
            return true;
        }

        return false;
    }


    public override bool InheritInteractionsFrom(Gizmo other)
    {
        if (settables == null)
        {
            settables = new List<IPlantToGrowSettable>();
        }

        settables.Add(((Command_SetPlantWithSecondaryToGrow)other).settable);
        return false;
    }


    private void WarnAsAppropriate(ThingDef plantDef)
    {
        if (plantDef.plant.sowMinSkill <= 0)
        {
            return;
        }

        foreach (var current in settable.Map.mapPawns.FreeColonistsSpawned)
        {
            if (current.skills.GetSkill(SkillDefOf.Plants).Level >= plantDef.plant.sowMinSkill && !current.Downed &&
                current.workSettings.WorkIsActive(WorkTypeDefOf.Growing))
            {
                return;
            }
        }

        Find.WindowStack.Add(new Dialog_MessageBox("NoGrowerCanPlant".Translate(plantDef.label,
            plantDef.plant.sowMinSkill
        ).CapitalizeFirst()));
    }


    private bool IsPlantAvailable(ThingDef plantDef)
    {
        var sowResearchPrerequisites = plantDef.plant.sowResearchPrerequisites;
        if (sowResearchPrerequisites == null)
        {
            return true;
        }

        foreach (var researchProjectDef in sowResearchPrerequisites)
        {
            if (researchProjectDef.IsFinished)
            {
                continue;
            }

            return false;
        }

        return true;
    }
}