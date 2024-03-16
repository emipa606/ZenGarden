using System.Collections.Generic;
using Verse;

namespace ZenGarden;

public sealed class ZenGardenMod : Mod
{
    private static List<ThingDef> secondaryPlants;


    public ZenGardenMod(ModContentPack content) : base(content)
    {
        LongEventHandler.ExecuteWhenFinished(AssignPlants);
    }

    public static List<ThingDef> SecondaryPlants
    {
        get
        {
            if (!secondaryPlants.NullOrEmpty())
            {
                return secondaryPlants;
            }

            Log.Error(
                "Zen Garden:: Secondary plants list isn't assigned, and will not allow orchard zones to grow plants.");
            return null;
        }
    }


    private void AssignPlants()
    {
        secondaryPlants = [];
        foreach (var plant in DefDatabase<ThingDef>.AllDefs)
        {
            if (plant.HasModExtension<SecondaryResource>())
            {
                secondaryPlants.Add(plant);
            }
        }
    }
}