﻿using System.Collections.Generic;

using RimWorld;
using Verse;
using Verse.AI;

namespace ZenGarden
{

    public class JobDriver_PlantsSowSecondary : JobDriver
    {

        private float sowWorkDone;

        private PlantWithSecondary Plant
        {
            get
            {
                return (PlantWithSecondary)job.GetTarget(TargetIndex.A).Thing;
            }
        }


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref sowWorkDone, "sowWorkDone", 0f, false);
        }


        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job);
        }


        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Reserve.Reserve(TargetIndex.A, 1, -1, null);
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.Touch).FailOn(() => PlantUtility.AdjacentSowBlocker(job.plantDefToSow, TargetA.Cell, Map) != null).FailOn(() => !job.plantDefToSow.CanEverPlantAt(TargetLocA, Map));
            Toil sowToil = new Toil()
            {
                initAction = delegate
                {
                    TargetThingA = GenSpawn.Spawn(job.plantDefToSow, TargetLocA, Map);
                    pawn.Reserve(TargetThingA, job);
                    PlantWithSecondary plant = (PlantWithSecondary)TargetThingA;
                    plant.Growth = 0f;
                    plant.sown = true;
                },
                tickAction = delegate
                {
                    Pawn actor = GetActor();
                    if (actor.skills != null)
                    {
                        actor.skills.Learn(SkillDefOf.Plants, 0.11f, false);
                    }
                    float statValue = actor.GetStatValue(StatDefOf.PlantWorkSpeed, true);
                    float num = statValue;
                    PlantWithSecondary plant = Plant;
                    if (plant.LifeStage != PlantLifeStage.Sowing)
                    {
                        Log.Error(this + " getting sowing work while not in Sowing life stage.");
                    }
                    sowWorkDone += num;
                    if (sowWorkDone >= plant.def.plant.sowWork)
                    {
                        plant.Growth = 0.05f;
                        plant.Sec_Growth = 0.05f;
                        Map.mapDrawer.MapMeshDirty(plant.Position, MapMeshFlag.Things);
                        actor.records.Increment(RecordDefOf.PlantsSown);
                        ReadyForNextToil();
                        return;
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Never
            };
            sowToil.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            sowToil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            sowToil.WithEffect(EffecterDefOf.Sow, TargetIndex.A);
            sowToil.WithProgressBar(TargetIndex.A, () => sowWorkDone / Plant.def.plant.sowWork, true, -0.5f);
            sowToil.PlaySustainerOrSound(() => SoundDefOf.Interact_Sow);
            sowToil.AddFinishAction(delegate
            {
                if (TargetThingA != null)
                {
                    PlantWithSecondary plant = (PlantWithSecondary)GetActor().CurJob.GetTarget(TargetIndex.A).Thing;
                    if (sowWorkDone < plant.def.plant.sowWork && !TargetThingA.Destroyed)
                    {
                        TargetThingA.Destroy(DestroyMode.Vanish);
                    }
                }
            });
            yield return sowToil;
        }
    }
}
