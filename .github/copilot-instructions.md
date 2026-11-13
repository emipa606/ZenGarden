# GitHub Copilot Instructions for Zen Garden (Continued) Mod

## Mod Overview and Purpose

**Zen Garden (Continued)** is an update to the original **cuproPandas** mod, based on **mantrasongs**' updated version. The mod aims to address reported issues from the previous versions and introduces new features and decorations to enhance the gardening experience in the RimWorld game. 

The mod adds various Zen and rock garden-themed decorations, such as unique wood types, fruits, trees, and ornamental objects. It includes an Orchard Zone for growing secondary resources and various gravel flooring options to improve aesthetics and functionality.

## Key Features and Systems

- **New Wood Types**: Cherry Wood (pink colored) and Ebony Wood (black colored) are introduced.
- **Secondary Resources**: Zen Cherries from the Zen Cherry Tree, Persimmons from the Persimmon Tree, and Crabapples from the Crabapple Tree.
- **Decorative Objects**: A wide range of decorative items like Border Path, Border Pond, Stacked Rocks, Artificial Hedges, Flower Arch, Scenic Bench, Fountain, and more.
- **Growing Zones**: Orchard Zone for harvesting secondary resources from trees.
- **Gravel Floors**: Rough Gravel and Gravel Floor for creating decorative paths and flooring.
- **Compatibility**: Works with other mods like Architect Sense, Kura's Extra Minerals, Cupro's Stones, Medieval Times, Survivalist's Additions, and SeedsPlease.

## Coding Patterns and Conventions

- **C# Files**: Located in the `Source\ZenGarden` directory.
- **XML Files**: Located in the `1.7\Defs` directory.
- **Harmony Patching**: Utilizes the Harmony library for patching existing RimWorld functionality.
- **Naming Conventions**: C# classes follow PascalCase, while methods and properties follow camelCase.
- **Comments**: Extensive comments are added to explain complex logic and important sections of the code.

## XML Integration

The mod uses XML files to define new game elements. These XML files are structured to fit the RimWorld modding API and are located in the `1.7\Defs` directory. Key XML files include:

- **DesignationCategoryDefs.xml**: Defines new designation categories.
- **TerrainDefs.xml**: Defines new terrain types.
- **ThingDefs_Items.xml**: Defines new items and usable objects.

Each XML file contains multiple `Def` entries, such as `DesignationCategoryDef`, `TerrainDef`, and `ThingDef`, which are used to register new game elements with RimWorld.

## Harmony Patching

Harmony patching is extensively used in the mod to modify existing RimWorld behavior. Key Harmony patches include:

- **JoyGiver_SitAtScenicBench**: Modifies the `TryGiveJob` method to add a new joy giver for colonists to sit at scenic benches.
- **JobDriver_SitAtScenicBench**: Modifies the `MakeNewToils` and `TryMakePreToilReservations` methods to enhance the job driver for sitting at scenic benches.
- **WorkGiver_GrowerHarvest_HasJobOnCell**: Modifies the `Postfix` method to adjust the behavior of the grower harvest work giver.

## Suggestions for Copilot

To effectively use GitHub Copilot for this mod:

1. **Understand the Mod Structure**: Familiarize yourself with the C# and XML file structures and the Harmony patching mechanism.
2. **Use Semantic Suggestions**: When working with XML files, Copilot can generate `Def` entries and their attributes based on existing patterns.
3. **Modify Existing Code**: For C# files, Copilot can suggest modifications to existing methods, such as adding new logic or changing existing behavior.
4. **Generate New Code**: Use Copilot to generate new classes and methods for adding new features or enhancing existing ones.
5. **Review and Test**: Always review the generated code for accuracy and test it within the RimWorld modding environment to ensure compatibility and functionality.

## Example Usage

### Generating a New XML Def

xml
<!-- Example of a new ThingDef -->
<ThingDef>
    <defName>ZenGarden_FlowerArch</defName>
    <label>Flower Arch</label>
    <description>A decorative flower arch used for garden paths and wedding spots.</description>
    <category>Building</category>
    <thingClass>Building_FlowerArch</thingClass>
    <statBases>
        <Beauty>5</Beauty>
    </statBases>
</ThingDef>


### Modifying an Existing C# Method

csharp
// Example of modifying an existing method in JoyGiver_SitAtScenicBench.cs
public override Job TryGiveJob(Pawn pawn)
{
    // Existing logic
    if (!pawn.Awake())
        return null;

    // Copilot suggested addition
    if (pawn.needs.mood == null)
        return null;

    // Rest of the method
}


### Generating a New Harmony Patch

csharp
// Example of a new Harmony patch in WorkGiver_GrowerHarvest_HasJobOnCell.cs
[HarmonyPatch(typeof(WorkGiver_GrowerHarvest_HasJobOnCell), "Postfix")]
public static class WorkGiver_GrowerHarvest_HasJobOnCell_Patch
{
    public static void Postfix(ref bool __result, WorkGiver_GrowerHarvest_HasJobOnCell __instance, ref Pawn pawn, ref IntVec3 c)
    {
        // Copilot suggested addition
        if (__result && pawn.Faction == Faction.OfPlayer)
        {
            __result = false;
        }
    }
}


By following these instructions and utilizing GitHub Copilot effectively, you can enhance the development process for the Zen Garden (Continued) mod and contribute to improving the gardening experience in RimWorld.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

