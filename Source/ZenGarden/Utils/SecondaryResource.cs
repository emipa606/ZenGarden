using System.Collections.Generic;
using RimWorld;
using Verse;

namespace ZenGarden;

public class SecondaryResource : DefModExtension
{
    // If the plant uses a different graphic when blooming
    public string bloomingGraphicPath;

    // Customize what biomes this is forbidden to grow in
    // Useful for fruits that should only grow in certain biomes
    // Note: This is only calculated by the orchard zone, not the normal growing zones
    public List<BiomeDef> forbiddenGrowBiomes;

    // Customize how quickly this grows and when it is first harvestable
    public float growDays = 4f;

    // What to harvest
    public ThingDef harvestedThingDef;

    // Customize what seasons this is allowed to grow in
    // Useful for fruits that should only grow in spring, etc.
    public List<Season> limitedGrowSeasons;

    public int maxToHarvest;

    // If an exact amount is desired, only put a value for maxToHarvest. minToHarvest gets checked, and if it's MaxValue only maxToHarvest is used
    public int minToHarvest = int.MaxValue;

    public float parentMinGrowth = 0.5f;

    // If SeedsPlease is installed, this is a seed that will drop when the secondary thing is harvested
    // This is a string because the ThingDef is added via a patch
    //public string seedsPleaseSeedDef;
    // If the thing harvested doesn't match up with what is growing (e.g. latex bucket vs latex), use a special label
    public string specialThingDefLabel;
}