# GitHub Copilot Instructions for Zen Garden (Continued) Mod

## Mod Overview and Purpose

Zen Garden (Continued) is a delightful mod for RimWorld that adds a variety of zen and rock garden-themed decorations to enhance the aesthetic and ambiance of your colony. The mod is a continuation and update of cuproPanda's original work, with further updates by mantrasongs. Users can create peaceful and visually appealing spaces within their RimWorld colonies by researching stonecutting, which unlocks all the new additions.

## Key Features and Systems

- **New Resources**: 
  - *Cherry Wood*: Pink-colored wood.
  - *Ebony Wood*: Black-colored wood.
  - *Zen Cherries*: Secondary resource from Zen Cherry Tree.
  - *Persimmon*: Secondary resource from Persimmon Tree.
  - *Crabapple*: Secondary resource from Crabapple Tree.

- **New Objects**: 
  - A variety of garden objects such as paths, ponds, hedges, walls, benches, fountains, and lanterns.
  - Unique items like Scenic Benches and Flower Arches that react dynamically to their environment.

- **Orchard Zone**: 
  - A special growing zone that focuses on harvesting secondary resources without cutting down the plants.

- **Customizable Gravel Floor**: 
  - Can be raked into various patterns to suit aesthetic needs.

## C# Coding Patterns and Conventions

### Classes and Methods

- **Job Drivers**
  - Classes such as `JobDriver_PlantsHarvestSecondary` and `JobDriver_SitAtScenicBench` extend from base job classes and handle specific tasks related to gardening and scenic benches.

- **Work Givers**
  - Distinct classes to manage plant sowing and harvesting through `WorkGiver_GrowerHarvestSecondary` and `WorkGiver_GrowerSowSecondary`, utilizing specific methods for plant locations and requirements.

- **Designators and Zones**
  - Specific classes such as `Designator_ZoneAdd_Orchard` are used for creating and managing new zones, in this case, the Orchard Zone.

- **Buildings and Structures**
  - Classes like `Building_Fountain` and `Building_FlowerArch` provide structural functionality and interaction in the game world.

### General Conventions

- Use PascalCase for class and method names.
- Private methods should be concise and descriptive, e.g., `HarvestableLocation`.
- Leverage base classes and inheritance to extend functionality while maintaining DRY principles.

## XML Integration

- XML files define the properties and settings for new items, plants, and objects added by the mod.
- XML schemas are leveraged to configure the appearance, interactions, and relationships of assets with the game environment.
- Ensure that XML files are organized within the appropriate directories (`Defs` folder) for seamless integration.

## Harmony Patching

- Harmony is used to patch methods and extend or modify existing game functionalities without altering core game files.
- Carefully construct patches for both `pre` and `post` method execution where behavior modifications are necessary.
- Ensure patches are conflict-free by checking for existing patches or modifications in the community.

## Suggestions for Copilot

- Provide detailed function headers and comments to guide AI autocompletion toward generating more accurate code snippets.
- Highlight specific conditional logic and method extensions where enhanced functionality is expected.
- Create templates for commonly used patterns, such as job driver instructions, to expedite the development of new features.

Overall, this mod emphasizes decoration and visual enhancement within the game, providing players with the tools to create uniquely styled gardens and outdoor areas, while integrating seamlessly into existing game mechanics through careful C# and XML development practices. Always remember to back up your game saves before installing new mods to avoid any potential issues.


This `.github/copilot-instructions.md` file aims to guide developers using GitHub Copilot in understanding and enhancing the Zen Garden mod. It encapsulates mod specifics, coding practices, and integration strategies to ensure a coherent and enriching development experience.
