using SFML.Graphics;
using SFML.System;
using System;
using TankGame.Src.Data;
using TankGame.Src.Events;

namespace TankGame.Src.Actors.GameObjects.Activities
{
    internal abstract class Activity : GameObject, ITickable
    {
        public string Name { get; }
        public string ProgressText => CalculateProgress();
        public uint PointsAdded { get; }
        public ActivityStatus ActivityStatus { get; protected set; }

        public Activity(Vector2i coords, Tuple<TraversibilityData, DestructabilityData> gameObjectType, Texture texture, string type, int hp, string name, uint pointsAdded) : base(coords, gameObjectType, texture, type, hp)
        {
            Name = name;
            PointsAdded = pointsAdded;
            ActivityStatus = ActivityStatus.NotStarted;
            RegisterTickable();
        }

        protected void ChangeStatus(ActivityStatus activityStatus)
        {
            ActivityStatus = activityStatus;
            if (activityStatus == ActivityStatus.Completed) GamestateManager.Instance.CompleteActivity(PointsAdded, Position + new Vector2f((Size.X / 2) - 75, (Size.Y / 10) - 10));
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
    }
}
