using RimWorld;
using UnityEngine;
using Verse;

namespace ZenGarden;

[StaticConstructorOnStartup]
internal class Building_FlowerArch : Building
{
    private static readonly Graphic GraphicPlain = GraphicDatabase.Get<Graphic_Single>(
        "Cupro/Object/FlowerArch/FlowerArch_Plain", ShaderDatabase.DefaultShader, new Vector2(2, 2), Color.white);

    private static readonly Graphic GraphicVines = GraphicDatabase.Get<Graphic_Single>(
        "Cupro/Object/FlowerArch/FlowerArch_Vines", ShaderDatabase.DefaultShader, new Vector2(2, 2), Color.white);

    private static readonly Graphic GraphicBlooming = GraphicDatabase.Get<Graphic_Single>(
        "Cupro/Object/FlowerArch/FlowerArch_Blooming", ShaderDatabase.DefaultShader, new Vector2(2, 2), Color.white);

    private static readonly Graphic GraphicFrozen = GraphicDatabase.Get<Graphic_Single>(
        "Cupro/Object/FlowerArch/FlowerArch_Frozen", ShaderDatabase.DefaultShader, new Vector2(2, 2), Color.white);

    public override Graphic Graphic
    {
        get
        {
            if (Map.weatherManager.SnowRate > 0)
            {
                return GraphicFrozen;
            }

            if (!(Temperature > 5f))
            {
                return GraphicPlain;
            }

            if (Season == Season.Spring || Season == Season.Summer)
            {
                return GraphicBlooming;
            }

            return GraphicVines;
        }
    }

    public float Temperature => !GenTemperature.TryGetTemperatureForCell(Position, Map, out var num) ? 1f : num;

    private Season Season => GenLocalDate.Season(Map);
}