using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Xml;
using TankGame.Src.Actors.Fields;
using TankGame.Src.Data;
using TankGame.Src.Data.Map;
using TankGame.Src.Events;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.GameObjects
{
    internal class GameObject : Actor, IDestructible
    {
        public TraversibilityData TraversibilityData { get; protected set; }
        public DestructabilityData DestructabilityData { get; protected set; }
        private DestructabilityData DestructabilityDataAfterDestruction { get; }
        private TraversibilityData TraversibilityDataAfterDestruction { get; }
        protected SpriteComponent ObjectSprite { get; set; }
        public Region Region { private get; set; }
        private string Type { get; }
        public int Health { get => DestructabilityData.Health; set => DestructabilityData = new DestructabilityData(value, DestructabilityData.IsDestructible, DestructabilityData.DestroyOnEntry); }
        public bool IsTraversible => TraversibilityData.IsTraversible;
        public bool IsDestructible => DestructabilityData.IsDestructible;
        public bool IsDestructibleOrTraversible => IsDestructible || IsTraversible;
        public bool IsAlive => Health > 0;
        public Actor Actor => this;
        public Field Field { get; set; }
        public Vector2i Coords => new Vector2i((int)(Position.X / Size.X), (int)(Position.Y / Size.Y));
        public Region CurrentRegion => Region ??= GamestateManager.Instance.Map.GetRegionFromFieldCoords(Coords);
        protected Texture AfterDestructionTexture { get; set; }
        public bool StopsProjectile => DestructabilityData.StopsProjectile;

        public GameObject(Vector2i coords, Tuple<TraversibilityData, DestructabilityData, string> gameObjectType, Texture texture, string type, int hp) : base(new Vector2f(coords.X * 64, coords.Y * 64), new Vector2f(64, 64))
        {
            TraversibilityData = gameObjectType.Item1;
            DestructabilityData = gameObjectType.Item2;

            DestructabilityDataAfterDestruction = new DestructabilityData(1, false, false, false);
            TraversibilityDataAfterDestruction = new TraversibilityData(1.25F, true);

            Type = type;
            AfterDestructionTexture = gameObjectType.Item3 == null ? null : AfterDestructionTexture = TextureManager.Instance.GetTexture("gameobject", gameObjectType.Item3); 

            if (hp > -1) Health = hp;

            ObjectSprite = Health == 0
                ? new SpriteComponent(Position, Size, this, AfterDestructionTexture, new Color(255, 255, 255, 255))
                : new SpriteComponent(Position, Size, this, texture, new Color(255, 255, 255, 255));

            RegisterDestructible();

            RenderLayer = RenderLayer.GameObject;
            RenderView = RenderView.Game;

        }

        public override HashSet<IRenderComponent> GetRenderComponents()
        {
            return new HashSet<IRenderComponent> { ObjectSprite };
        }

        public virtual void OnDestroy()
        {
            if (AfterDestructionTexture == null)
            {
                Field.OnGameObjectDestruction();
                Dispose();
            }
            else
            {
                ObjectSprite = new SpriteComponent(Position, Size, this, AfterDestructionTexture, new Color(255, 255, 255, 255));
                TraversibilityData = TraversibilityDataAfterDestruction;
                DestructabilityData = DestructabilityDataAfterDestruction;
            }
        }

        public void OnHit()
        {
            SoundManager.Instance.PlayRandomSound("destruction", Position / 64);
            if (IsDestructible && IsAlive) Health--;
            if (Health <= 0) OnDestroy();
        }

        internal XmlElement SerializeToXML(XmlDocument xmlDocument)
        {
            XmlElement objectElement = xmlDocument.CreateElement("object");
            XmlElement typeElement = xmlDocument.CreateElement("type");
            XmlElement hpElement = xmlDocument.CreateElement("hp");

            typeElement.InnerText = Type;
            hpElement.InnerText = DestructabilityData.Health.ToString();

            objectElement.AppendChild(typeElement);
            objectElement.AppendChild(hpElement);

            return objectElement;
        }

        public override void Dispose()
        {
            UnregisterDestructible();
            base.Dispose();
        }

        public void RegisterDestructible()
        {
            MessageBus.Instance.PostEvent(MessageType.RegisterDestructible, this, new EventArgs());
        }

        public void UnregisterDestructible()
        {
            MessageBus.Instance.PostEvent(MessageType.UnregisterDestructible, this, new EventArgs());
        }
    }
}
