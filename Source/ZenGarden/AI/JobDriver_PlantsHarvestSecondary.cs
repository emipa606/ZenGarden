using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace ZenGarden;

public class JobDriver_PlantsHarvestSecondary : JobDriver_PlantWork
{
    private float workDone;

    protected new PlantWithSecondary Plant => (PlantWithSecondary)job.GetTarget(TargetIndex.A).Thing;

    protected override IEnumerable<Toil> MakeNewToils()
    {
        Init();

        yield return Toils_JobTransforms.MoveCurrentTargetIntoQueue(TargetIndex.A);

        var initExtractTargetFromQueue = Toils_JobTransforms.ClearDespawnedNullOrForbiddenQueuedTargets(TargetIndex.A);
        yield return initExtractTargetFromQueue;

        yield return Toils_JobTransforms.SucceedOnNoTargetInQueue(TargetIndex.A);
        yield return Toils_JobTransforms.ExtractNextTargetFromQueue(TargetIndex.A);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch)
            .JumpIfDespawnedOrNullOrForbidden(TargetIndex.A, initExtractTargetFromQueue);

        var cut = new Toil();

        cut.tickAction = delegate
        {
            var actor = cut.actor;
            actor.skills?.Learn(SkillDefOf.Plants, 0.11f);

            workDone += WorkDonePerTick(actor, Plant);
            if (workDone < Plant.def.plant.harvestWork)
            {
                return;
            }

            if (Plant.def.plant.harvestedThingDef != null)
            {
                if (actor.RaceProps.Humanlike && Plant.def.plant.harvestFailable &&
                    Rand.Value > actor.GetStatValue(StatDefOf.PlantHarvestYield))
                {
                    MoteMaker.ThrowText((pawn.DrawPos + Plant.DrawPos) / 2f, Map, "TextMote_HarvestFailed".Translate(),
                        3.65f);
                }
                else
                {
                    var thing = Plant.CollectSecondaryThing();
                    if (actor.Faction != Faction.OfPlayer)
                    {
                        thing.SetForbidden(true);
                    }

                    GenPlace.TryPlaceThing(thing, actor.Position, Map, ThingPlaceMode.Near);
                    actor.records.Increment(RecordDefOf.PlantsHarvested);
                }
            }

            Plant.def.plant.soundHarvestFinish.PlayOneShot(actor);
            workDone = 0f;
            ReadyForNextToil();
        };
        cut.FailOn(() => !Plant.Sec_HarvestableNow);
        cut.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        cut.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
        cut.defaultCompleteMode = ToilCompleteMode.Never;
        cut.WithEffect(EffecterDefOf.Harvest_Tree, TargetIndex.A);
        cut.WithProgressBar(TargetIndex.A, () => workDone / Plant.def.plant.harvestWork, true);
        cut.PlaySustainerOrSound(() => Plant.def.plant.soundHarvesting);
        cut.activeSkill = () => SkillDefOf.Plants;
        yield return cut;

        yield return Toils_General.RemoveDesignationsOnThing(TargetIndex.A,
            ZenDefOf.ZEN_Designator_PlantsHarvestSecondary);

        yield return Toils_Jump.Jump(initExtractTargetFromQueue);
    }
}