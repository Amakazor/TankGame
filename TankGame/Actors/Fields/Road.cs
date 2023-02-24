using System.Collections.Generic;
using LanguageExt;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Fields.Roads;
using TankGame.Actors.GameObjects;
using TankGame.Core.Textures;

namespace TankGame.Actors.Fields; 

public class Road : Field {
    private static readonly HashMap<Directions, Texture> TexMap = new Dictionary<Directions, Texture>() {
        {Directions.None,    TextureManager.Get(TextureType.Field, "road")},
        {Directions.Top,     TextureManager.Get(TextureType.Field, "road-U")},
        {Directions.Bottom,  TextureManager.Get(TextureType.Field, "road-D")},
        {Directions.Left,    TextureManager.Get(TextureType.Field, "road-L")},
        {Directions.Right,   TextureManager.Get(TextureType.Field, "road-R")},
        {Directions.Top    | Directions.Left,    TextureManager.Get(TextureType.Field, "road-UL")},
        {Directions.Top    | Directions.Right,   TextureManager.Get(TextureType.Field, "road-UR")},
        {Directions.Bottom | Directions.Left,    TextureManager.Get(TextureType.Field, "road-DL")},
        {Directions.Bottom | Directions.Right,   TextureManager.Get(TextureType.Field, "road-RD")},
        {Directions.Top    | Directions.Bottom,  TextureManager.Get(TextureType.Field, "road-UD")},
        {Directions.Left   | Directions.Right,   TextureManager.Get(TextureType.Field, "road-RL")},
        {Directions.Top    | Directions.Bottom | Directions.Left,  TextureManager.Get(TextureType.Field, "road-UDL")},
        {Directions.Top    | Directions.Bottom | Directions.Right, TextureManager.Get(TextureType.Field, "road-URD")},
        {Directions.Top    | Directions.Left   | Directions.Right, TextureManager.Get(TextureType.Field, "road-URL")},
        {Directions.Bottom | Directions.Left   | Directions.Right, TextureManager.Get(TextureType.Field, "road-RDL")},
        {Directions.Top    | Directions.Bottom | Directions.Left | Directions.Right, TextureManager.Get(TextureType.Field, "road-URDL")}
    }.ToHashMap();

    static Road() { }
    
    public new class Dto : Field.Dto { }

    public Road(Vector2i coords, Option<GameObject> gameObject) : base(coords, TexMap[Directions.None], gameObject) { }
    public Road(Dto dto, Vector2i coords) : base(dto, coords, TexMap[Directions.None]) { }

    public override float BaseSpeedModifier => 1f;
    public override bool BaseTraversible => true;

    public override void PostProcess() { 
        base.PostProcess();

        var neighbours = Neighbours();
        var directionFlag = Seq(Directions.Bottom, Directions.Left, Directions.Right, Directions.Top)
                  .Filter(flag => neighbours[flag].Map(neighbour => neighbour is Road).IfNone(false))
                  .Reduce((current, filteredFlag) => current | filteredFlag);
        
        CreateSurface(TexMap[directionFlag]);
    }
}