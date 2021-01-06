using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TankGame.Src.Data;
using TankGame.Src.Extensions;
using TankGame.Src.Pathfinding;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    internal class PatrolAIMovementController : AIMovementController
    {
        protected List<Vector2i> PatrolRoute { get; }
        protected Stack<Vector2i> CurrentPatrolRoute { get; set; }
        protected Vector2i TargetPosition { get; set; }
        protected Stack<Node> Path { get; set; }

        public PatrolAIMovementController(double delay, Pawn owner, List<Vector2i> patrolRoute) : base(delay, owner, "patrol")
        {
            PatrolRoute = patrolRoute;
            TargetPosition = new Vector2i(-1, -1);
        }

        protected override void DecideOnNextAction()
        {
            if (CanDoAction() && Owner.CurrentRegion != null)
            {
                if (CanSeePlayerInUnobstructedLine || CanSeeActivityInUnobstructedLine) NextAction = KeyActionType.Shoot;
                else
                {
                    if (CurrentPatrolRoute is null || CurrentPatrolRoute.Count == 0)
                    {
                        CurrentPatrolRoute = new Stack<Vector2i>(PatrolRoute);
                    }

                    if (TargetPosition.IsInvalid() || TargetPosition.Equals(Owner.Coords))
                    {
                        TargetPosition = CurrentPatrolRoute.Pop();
                    }

                    if (!TargetPosition.IsInvalid())
                    {
                        if (Path != null && Path.Count == 0) Path = null;

                        Path ??= GeneratePath(GamestateManager.Instance.Map.GetNodesInRadius(Owner.Coords, BaseSightDistance), new Vector2i(BaseSightDistance, BaseSightDistance), new Vector2i(BaseSightDistance, BaseSightDistance) + TargetPosition - Owner.Coords);

                        NextAction = Path == null ? null : GetActionFromNextCoords(Path.Pop().Position + Owner.Coords - new Vector2i(BaseSightDistance, BaseSightDistance));
                    }
                    else NextAction = null;
                }
            }
            else NextAction = null;
        }

        internal XmlNode SerializePath(XmlDocument xmlDocument)
        {
            XmlElement pathElement = xmlDocument.CreateElement("path");

            foreach (Vector2i point in PatrolRoute)
            {
                XmlElement pointElement = xmlDocument.CreateElement("point");
                XmlElement xElement = xmlDocument.CreateElement("x");
                XmlElement yElement = xmlDocument.CreateElement("y");

                xElement.InnerText = point.X.ToString();
                yElement.InnerText = point.Y.ToString();

                pointElement.AppendChild(xElement);
                pointElement.AppendChild(yElement);

                pathElement.AppendChild(pointElement);
            }

            return pathElement;
        }
    }
}
