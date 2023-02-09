using System.Collections.Generic;
using System.Text.Json.Serialization;
using LanguageExt;
using SFML.System;
using TankGame.Actors.Brains.Thoughts;
using TankGame.Actors.Pawns;
using TankGame.Core.Gamestates;
using TankGame.Extensions;
using TankGame.Pathfinding;

namespace TankGame.Actors.Brains.Goals; 

public class ChasePlayerGoal : Goal {
    public new class Dto : Goal.Dto {
        [JsonIgnore] public Option<Vector2i> CachedOwnerPosition { get; set; }
        public Vector2i? OwnerPosition {
            get => CachedOwnerPosition.MatchUnsafe<Vector2i?>(x => x, () => null);
            set => CachedOwnerPosition = Optional(value);
        }
        
        [JsonIgnore] public Option<Vector2i> CachedPlayerPosition { get; set; }
        public Vector2i? PlayerPosition {
            get => CachedPlayerPosition.MatchUnsafe<Vector2i?>(x => x, () => null);
            set => CachedPlayerPosition = Optional(value);
        }
        public Stack<Node> Path { get; set; }
    }
    
    public ChasePlayerGoal(Brain brain) : base(brain) { }
    
    public ChasePlayerGoal(Brain brain, Dto dto) : base(brain, dto) {
        CachedOwnerPosition = dto.CachedOwnerPosition;
        CachedPlayerPosition = dto.CachedPlayerPosition;
        Path = dto.Path;
    }

    private Option<Vector2i> CachedOwnerPosition { get; set; }
    private Option<Vector2i> CachedPlayerPosition { get; set; }
    private Stack<Node> Path { get; set; } = new();
    
    public override Option<Thought> NextThought() {
        return Gamestate.Player.Match<Option<Thought>>(
            player => {
                ValidatePath();
                if (player.Coords.SquareEuclideanDistance(Brain.Owner.Coords) > Brain.Owner.SquareSightDistance) return null;
                switch (Path.Count) {
                    case 0:
                        CachedPlayerPosition = player.Coords;
                        CachedOwnerPosition = Brain.Owner.Coords;
                        Path = AStar.FindPath(Gamestate.Level.GetNodesInRadius(Brain.Owner.Coords, Brain.Owner.SightDistance), Brain.Owner.Coords, player.Coords);
                        goto default;
                    default:
                        return CachedPlayerPosition.Match(GetThought, None);
                }
            }, None);
    }

    private Option<Thought> GetThought(Vector2i position) {
        Direction newDirection = Path.Peek().GetDirectionFrom(position);
        if (newDirection != Brain.Owner.Direction) return new RotateThought(Brain, 1, newDirection);

        var moveThought = Gamestate.Level.FieldAt(Brain.Owner.Coords)
              .SelectMany(_ => Gamestate.Level.FieldAt(Path.Peek().Coords), (baseField, targetField) => new { baseField, targetField })
              .Where(fields => fields.targetField.Traversible)
              .Select(fields => new MoveThought(Brain, 1, fields.baseField, fields.targetField));

        if (moveThought.IsNone) return None;

        Path.Pop();
        return moveThought.Map<Thought>(t => t);
    }

    private void ValidatePath() {
        if (Path.Count > 0 && !CachedPlayerPositionIsUpToDate()) Path.Clear();
        if (Path.Count > 0 && !CheckAndUpdateCachedOwnerPosition()) Path.Clear();
    }

    private bool CachedPlayerPositionIsUpToDate()
        => CachedPlayerPosition.SelectMany(_ => Gamestate.Player, (playerPosition, player) => playerPosition == player.Coords).IfNone(false);

    private bool CheckAndUpdateCachedOwnerPosition() {
        if (CachedOwnerPosition.IsNone) return false;

        var ownerCoords = Brain.Owner.Coords;
        if (CachedOwnerPosition == ownerCoords) return true;
        
        CachedOwnerPosition = ownerCoords;
        return true;
    }

    public override Dto ToDto()
        => new() { Id = Id, CachedOwnerPosition = CachedOwnerPosition, CachedPlayerPosition = CachedPlayerPosition, Path = Path, };
}