using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.Fields
{
    internal class Field : Actor
    {
        public Vector2i Coords { get; }
        public float SpeedModifier { get; }
        public bool IsTraversible { get; }

        private SpriteComponent Surface { get; }

        public Field(Vector2i coords, float speedModifier, bool isTraversible, Texture texture) : base(new Vector2f(coords.X * 64, coords.Y * 64), new Vector2f(64, 64))
        {
            Coords = coords;
            SpeedModifier = speedModifier;
            IsTraversible = isTraversible;

            Surface = new SpriteComponent(Position, Size, this, texture, new Color());
        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { Surface };
        }
    }
}
