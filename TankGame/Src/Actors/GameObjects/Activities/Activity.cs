using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using TankGame.Src.Actors.Pawns.Enemies;
using TankGame.Src.Data;
using TankGame.Src.Events;

namespace TankGame.Src.Actors.GameObjects.Activities
{
    internal abstract class Activity : GameObject, ITickable
    {
        protected uint AllEnemiesCount;
        public string Name { get; }
        public string ProgressText => CalculateProgress();
        public int PointsAdded { get; protected set; }
        public ActivityStatus ActivityStatus { get; protected set; }
        protected HashSet<Enemy> Enemies { get; set; }

        public Activity(Vector2i coords, HashSet<Enemy> enemies, int hp, string name, Tuple<TraversibilityData, DestructabilityData> gameObjectType, int pointsAdded) : base(coords, gameObjectType, TextureManager.Instance.GetTexture(TextureType.Activity, "tower"), "", hp)
        {
            Name = name;
            PointsAdded = pointsAdded;
            Enemies = enemies;
            ActivityStatus = ActivityStatus.Stopped;
            RegisterTickable();
        }

        public virtual void ChangeStatus(ActivityStatus activityStatus)
        {
            ActivityStatus = activityStatus;
            if (activityStatus == ActivityStatus.Completed) GamestateManager.Instance.CompleteActivity(PointsAdded, Position + new Vector2f((Size.X / 2) - 75, (Size.Y / 10) - 10));
            if (activityStatus == ActivityStatus.Failed) GamestateManager.Instance.FailActivity(PointsAdded, Position + new Vector2f((Size.X / 2) - 75, (Size.Y / 10) - 10));
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

        public override void OnDestroy()
        {
            GamestateManager.Instance.Map.GetRegionFromFieldCoords(Coords).Activity = null;
            base.OnDestroy();
        }
    }
}
