using System.Collections.Generic;
using System.Linq;
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

        foreach (var thingDef in containedAndAdjacentThings.Select(thing => thing.def))
        {
            if (thingDef.category != ThingCategory.Building)
            {
                continue;
            }

            if (ValidZenBuildings.Contains(thingDef.defName))
            {
                num++;
            }
        }

        return num;
    }
}