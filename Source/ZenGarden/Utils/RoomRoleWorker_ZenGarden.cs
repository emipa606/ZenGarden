using System.Collections.Generic;
using Verse;

namespace ZenGarden;

public class RoomRoleWorker_ZenGarden : RoomRoleWorker
{
    private static readonly List<string> ValidZenBuildings =
        ["ZEN_BorderPath", "ZEN_BorderPond", "ZEN_GravelCurve", "ZEN_GravelHoriz", "ZEN_GravelVert", "ZEN_Hedge"];

    public override float GetScore(Room room)
    {
        var num = 0;
        var containedAndAdjacentThings = room.ContainedAndAdjacentThings;

        foreach (var thing in containedAndAdjacentThings)
        {
            if (thing.def.category != ThingCategory.Building)
            {
                continue;
            }

            if (ValidZenBuildings.Contains(thing.def.defName))
            {
                num++;
            }
        }

        return num;
    }
}