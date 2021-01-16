using SFML.System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TankGame.Src.Data.Controls;
using TankGame.Src.Extensions;

namespace TankGame.Src.Actors.Pawns.MovementControllers
{
    internal class PatrolAIMovementController : AIMovementController
    {
        protected List<Vector2i> PatrolRoute { get; }
        protected Stack<Vector2i> CurrentPatrolRoute { get; set; }

        public PatrolAIMovementController(double delay, Pawn owner, List<Vector2i> patrolRoute) : base(delay, owner, "patrol")
        {
            PatrolRoute = patrolRoute;
            TargetPosition = new Vector2i(-1, -1);
        }

        protected override void DecideOnNextAction()
        {
            if (CanDoAction() && Owner.CurrentRegion != null)
            {
                if (CanSeePlayerInUnobstructedLine || (CanSeeActivityInUnobstructedLine && Owner.CurrentRegion != null && Owner.CurrentRegion.HasDestructibleActivity)) NextAction = KeyActionType.Shoot;
                else Patrol();

            }
            else NextAction = null;
        }

        protected void Patrol()
        {
            if (CurrentPatrolRoute is null || !CurrentPatrolRoute.Any()) CurrentPatrolRoute = new Stack<Vector2i>(PatrolRoute);

            if (TargetPosition.IsInvalid() || TargetPosition.Equals(Owner.Coords)) TargetPosition = CurrentPatrolRoute.Pop();

            if (!TargetPosition.IsInvalid())
            {
                if (Path != null && !Path.Any()) Path = null;

                Path ??= GeneratePath(Owner.CurrentRegion.GetNodesInRegion(), Owner.CurrentRegion.ConvertMapCoordsToRegionFieldCoords(Owner.Coords), Owner.CurrentRegion.ConvertMapCoordsToRegionFieldCoords(TargetPosition));

                NextAction = Path == null ? null : GetActionFromNextCoords(Path.Pop().Position + Owner.CurrentRegion.Coords * Owner.CurrentRegion.FieldsInLine);
            }
            else NextAction = null;
        }

        public XmlNode SerializePath(XmlDocument xmlDocument)
        {
            XmlElement pathElement = xmlDocument.CreateElement("path");

            foreach (Vector2i point in PatrolRoute)
            {
                XmlElement pointElement = xmlDocument.CreateElement("point");
                XmlElement xElement = xmlDocument.CreateElement("x");
                XmlElement yElement = xmlDocument.CreateElement("y");

                xElement.InnerText = (point.X % 20).ToString();
                yElement.InnerText = (point.Y % 20).ToString();

                pointElement.AppendChild(xElement);
                pointElement.AppendChild(yElement);

                pathElement.AppendChild(pointElement);
            }

            return pathElement;
        }
    }
}
