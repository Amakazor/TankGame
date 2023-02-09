using System.Collections.Generic;
using LanguageExt;
using SFML.Graphics;
using SFML.System;
using TankGame.Actors.GameObjects;
using TankGame.Core.Textures;

namespace TankGame.Actors.Fields.Roads; 

public class Road : Field {
    private static readonly Dictionary<RoadDirection, Texture> TexMap;

    static Road() {
        TexMap = new() {
            {RoadDirection.None,  TextureManager.Get(TextureType.Field, "road")},
            {RoadDirection.Up,    TextureManager.Get(TextureType.Field, "road-U")},
            {RoadDirection.Down,  TextureManager.Get(TextureType.Field, "road-D")},
            {RoadDirection.Left,  TextureManager.Get(TextureType.Field, "road-L")},
            {RoadDirection.Right, TextureManager.Get(TextureType.Field, "road-R")},
            {RoadDirection.Up   | RoadDirection.Left,  TextureManager.Get(TextureType.Field, "road-UL")},
            {RoadDirection.Up   | RoadDirection.Right, TextureManager.Get(TextureType.Field, "road-UR")},
            {RoadDirection.Down | RoadDirection.Left,  TextureManager.Get(TextureType.Field, "road-DL")},
            {RoadDirection.Down | RoadDirection.Right, TextureManager.Get(TextureType.Field, "road-RD")},
            {RoadDirection.Up   | RoadDirection.Down,  TextureManager.Get(TextureType.Field, "road-UD")},
            {RoadDirection.Left | RoadDirection.Right, TextureManager.Get(TextureType.Field, "road-RL")},
            {RoadDirection.Up   | RoadDirection.Down | RoadDirection.Left,  TextureManager.Get(TextureType.Field, "road-UDL")},
            {RoadDirection.Up   | RoadDirection.Down | RoadDirection.Right, TextureManager.Get(TextureType.Field, "road-URD")},
            {RoadDirection.Up   | RoadDirection.Left | RoadDirection.Right, TextureManager.Get(TextureType.Field, "road-URL")},
            {RoadDirection.Down | RoadDirection.Left | RoadDirection.Right, TextureManager.Get(TextureType.Field, "road-RDL")},
            {RoadDirection.Up   | RoadDirection.Down | RoadDirection.Left | RoadDirection.Right, TextureManager.Get(TextureType.Field, "road-URDL")}
        };
    }
    
    public new class Dto : Field.Dto { }

    public Road(Vector2i coords, Option<GameObject> gameObject) : base(coords, TexMap[RoadDirection.None], gameObject) { }
    public Road(Dto dto, Vector2i coords) : base(dto, coords, TexMap[RoadDirection.None]) { }

    public override float BaseSpeedModifier => 1f;
    public override bool BaseTraversible => true;
}