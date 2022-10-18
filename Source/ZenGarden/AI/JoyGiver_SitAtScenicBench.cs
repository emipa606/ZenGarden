using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ZenGarden;

internal class JoyGiver_SitAtScenicBench : JoyGiver
{
    public override Job TryGiveJob(Pawn pawn)
    {
        // If the weather outside is frightful, only look for benches indoors
        var allowedOutside = JoyUtility.EnjoyableOutsideNow(pawn);

        // If the pawn doesn't have the required needs for some reason, fail
        if (pawn.needs == null || pawn.needs.joy == null || pawn.needs.mood == null || pawn.needs.mood.thoughts == null)
        {
            return null;
        }

        // Find a bench matching the correct criteria
        var bench = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial).Where(b =>
            b.def == ThingDef.Named("ZEN_ScenicBench") && b.Faction == Faction.OfPlayer && !b.IsForbidden(pawn)
            && (allowedOutside || b.Position.Roofed(b.Map)) &&
            pawn.CanReserveAndReach(b, PathEndMode.Touch, Danger.None));
        return !bench.TryRandomElementByWeight(delegate(Thing x)
        {
            var lengthHorizontal = (x.Position - pawn.Position).LengthHorizontal;
            return Mathf.Max(150f - lengthHorizontal, 5f);
        }, out var t)
            ? null
            : new Job(def.jobDef, t);
    }
}