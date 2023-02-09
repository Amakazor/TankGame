using System.Collections.Generic;
using LanguageExt;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.Fields.Roads;
using TankGame.Actors.GameObjects;
using TankGame.Core.Textures;

namespace TankGame.Actors.Fields; 

public class Road : Field {
    private static readonly HashMap<DirectionFlag, Texture> TexMap;

    static Road() {
        TexMap = new Dictionary<DirectionFlag, Texture>() {
            {DirectionFlag.None,    TextureManager.Get(TextureType.Field, "road")},
            {DirectionFlag.Top,     TextureManager.Get(TextureType.Field, "road-U")},
            {DirectionFlag.Bottom,  TextureManager.Get(TextureType.Field, "road-D")},
            {DirectionFlag.Left,    TextureManager.Get(TextureType.Field, "road-L")},
            {DirectionFlag.Right,   TextureManager.Get(TextureType.Field, "road-R")},
            {DirectionFlag.Top    | DirectionFlag.Left,    TextureManager.Get(TextureType.Field, "road-UL")},
            {DirectionFlag.Top    | DirectionFlag.Right,   TextureManager.Get(TextureType.Field, "road-UR")},
            {DirectionFlag.Bottom | DirectionFlag.Left,    TextureManager.Get(TextureType.Field, "road-DL")},
            {DirectionFlag.Bottom | DirectionFlag.Right,   TextureManager.Get(TextureType.Field, "road-RD")},
            {DirectionFlag.Top    | DirectionFlag.Bottom,  TextureManager.Get(TextureType.Field, "road-UD")},
            {DirectionFlag.Left   | DirectionFlag.Right,   TextureManager.Get(TextureType.Field, "road-RL")},
            {DirectionFlag.Top    | DirectionFlag.Bottom | DirectionFlag.Left,  TextureManager.Get(TextureType.Field, "road-UDL")},
            {DirectionFlag.Top    | DirectionFlag.Bottom | DirectionFlag.Right, TextureManager.Get(TextureType.Field, "road-URD")},
            {DirectionFlag.Top    | DirectionFlag.Left   | DirectionFlag.Right, TextureManager.Get(TextureType.Field, "road-URL")},
            {DirectionFlag.Bottom | DirectionFlag.Left   | DirectionFlag.Right, TextureManager.Get(TextureType.Field, "road-RDL")},
            {DirectionFlag.Top    | DirectionFlag.Bottom | DirectionFlag.Left | DirectionFlag.Right, TextureManager.Get(TextureType.Field, "road-URDL")}
        }.ToHashMap();
    }
    
    public new class Dto : Field.Dto { }

    public Road(Vector2i coords, Option<GameObject> gameObject) : base(coords, TexMap[DirectionFlag.None], gameObject) { }
    public Road(Dto dto, Vector2i coords) : base(dto, coords, TexMap[DirectionFlag.None]) { }

    public override float BaseSpeedModifier => 1f;
    public override bool BaseTraversible => true;

    public override void PostProcess() { 
        base.PostProcess();

        var neighbours = Neighbours();
        var directionFlag = Seq(DirectionFlag.Bottom, DirectionFlag.Left, DirectionFlag.Right, DirectionFlag.Top)
                  .Filter(flag => neighbours[flag].Map(neighbour => neighbour is Road).IfNone(false))
                  .Reduce((current, filteredFlag) => current | filteredFlag);
        
        CreateSurface(TexMap[directionFlag]);
    }
}