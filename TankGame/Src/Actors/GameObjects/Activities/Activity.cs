using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Xml;
using TankGame.Src.Actors.Data;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Data.Gamestate;
using TankGame.Src.Data.Textures;
using TankGame.Src.Events;
using TankGame.Src.Gui.RenderComponents;

namespace TankGame.Src.Actors.GameObjects.Activities
{
    internal abstract class Activity : GameObject, ITickable
    {
        protected uint AllEnemiesCount { get; set; }
        protected string Type { get; }

        private string name;
        public string Name
        {
            get => ActivityStatus switch
            {
                ActivityStatus.Started => name,
                ActivityStatus.Completed => "Completed",
                ActivityStatus.Failed => "Failed",
                _ => "",
            };

            set => name = value;
        }

        public string ProgressText => CalculateProgress();
        public int PointsAdded { get; protected set; }
        public ActivityStatus ActivityStatus { get; protected set; }
        protected HashSet<Enemy> Enemies { get; set; }
        protected Texture AfterCompletionTexture { get; }
        protected DestructabilityData AfterCompletionDestructabilityData { get; }

        protected Activity(Vector2i coords, HashSet<Enemy> enemies, int hp, string name, string type, Tuple<TraversibilityData, DestructabilityData, string> gameObjectType, int pointsAdded) : base(coords, gameObjectType, TextureManager.Instance.GetTexture(TextureType.GameObject, "tower"), "", hp)
        {
            Name = name;
            Type = type;
            PointsAdded = pointsAdded;
            Enemies = enemies;

            ActivityStatus = Health == 0 ? ActivityStatus.Failed : ActivityStatus.Stopped;

            AfterCompletionTexture = TextureManager.Instance.GetTexture("gameobject", "towercompleted");
            AfterCompletionDestructabilityData = new DestructabilityData(1, false, false);

            RegisterTickable();
        }

        public virtual void ChangeStatus(ActivityStatus activityStatus)
        {
            if (ActivityStatus != ActivityStatus.Failed && ActivityStatus != activityStatus)
            {
                ActivityStatus = activityStatus;
                if (activityStatus == ActivityStatus.Completed)
                {
                    GamestateManager.Instance.CompleteActivity(PointsAdded, Position + new Vector2f((Size.X / 2) - 75, (Size.Y / 10) - 10));
                    ChangeToCompleted();
                }

                if (activityStatus == ActivityStatus.Failed) GamestateManager.Instance.FailActivity(PointsAdded / 4, Position + new Vector2f((Size.X / 2) - 75, (Size.Y / 10) - 10));

                if (GamestateManager.Instance.Map != null) GamestateManager.Instance.Save();
            }
        }

        public void ChangeToCompleted()
        {
            ObjectSprite = new SpriteComponent(Position, Size, AfterCompletionTexture, new Color(255, 255, 255, 255));
            DestructabilityData = AfterCompletionDestructabilityData;
        }

        public void Tick(float deltaTime)
        {
            CalculateProgress();
        }

        public void RegisterTickable()
        {
            MessageBus.Instance.PostEvent(MessageType.RegisterTickable, this, new EventArgs());
        }

        public void UnregisterTickable()
        {
            MessageBus.Instance.PostEvent(MessageType.UnregisterTickable, this, new EventArgs());
        }

        protected abstract string CalculateProgress();

        public override void Dispose()
        {
            UnregisterTickable();
            base.Dispose();
        }

        public void OnEnemyWanderIn()
        {
            if (ActivityStatus != ActivityStatus.Completed) AllEnemiesCount++;
        }

        public void OnEnemyWanderOut()
        {
            if (ActivityStatus != ActivityStatus.Completed) AllEnemiesCount--;
        }

        internal abstract override XmlElement SerializeToXML(XmlDocument xmlDocument);
    }
}