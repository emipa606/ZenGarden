using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace ZenGarden;

[StaticConstructorOnStartup]
public class Building_Fountain : Building
{
    public static Graphic[] AnimFrames;
    private readonly int FrameCount = 4;
    public Graphic animOff;
    private Graphic currFrame;
    private int frameLerp;
    public CompPowerTrader powerComp;

    private TickManager tickMan;


    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        tickMan = Find.TickManager;
        powerComp = GetComp<CompPowerTrader>();
        LongEventHandler.ExecuteWhenFinished(GetGraphicArray);
    }


    public void GetGraphicArray()
    {
        //Creating frame array
        AnimFrames = new Graphic[FrameCount];
        for (var i = 0; i < FrameCount; i++)
        {
            AnimFrames[i] = GraphicDatabase.Get<Graphic_Single>($"Cupro/Anim/FountainWater{i + 1}",
                ShaderDatabase.Transparent, def.graphicData.drawSize, new Color(1f, 1f, 1f, 0.65f));
        }

        if (AnimFrames.Length > 0)
        {
            currFrame = AnimFrames.FirstOrDefault();
        }

        animOff = GraphicDatabase.Get<Graphic_Single>("Cupro/Anim/FountainWater0", ShaderDatabase.Transparent,
            def.graphicData.drawSize, new Color(1f, 1f, 1f, 0.65f));
    }


    public override void Tick()
    {
        base.Tick();

        if (tickMan.TicksGame % 35 != 0 || !powerComp.PowerOn)
        {
            return;
        }

        frameLerp++;
        if (frameLerp >= FrameCount)
        {
            frameLerp = 0;
        }

        currFrame = AnimFrames[frameLerp];
    }

    protected override void DrawAt(Vector3 drawLoc, bool flip = false)
    {
        base.DrawAt(drawLoc, flip);
        if (currFrame == null || animOff == null)
        {
            return;
        }

        var matrix = default(Matrix4x4);
        var s = new Vector3(def.graphicData.drawSize.x, 9f, def.graphicData.drawSize.y); //Size and altitude
        var x = new Vector3(0f, 0f, 0f); // This is an offset for drawing position
        matrix.SetTRS(drawLoc + x + Altitudes.AltIncVect, Rotation.AsQuat, s);
        if (powerComp.PowerOn && AnimFrames.Length > 0)
        {
            Graphics.DrawMesh(MeshPool.plane10, matrix, currFrame.MatAt(Rotation), 0);
        }
        else
        {
            Graphics.DrawMesh(MeshPool.plane10, matrix, animOff.MatAt(Rotation), 0);
        }
    }
}