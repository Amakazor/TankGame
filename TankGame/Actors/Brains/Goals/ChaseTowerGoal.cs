﻿using System.Collections.Generic;
using System.Text.Json.Serialization;
using LanguageExt;
using SFML.System;
using TankGame.Actors.Brains.Thoughts;
using TankGame.Actors.Pawns;
using TankGame.Core.Gamestates;
using TankGame.Extensions;
using TankGame.Pathfinding;

namespace TankGame.Actors.Brains.Goals; 

public class ChaseTowerGoal : Goal {
    public new class Dto : Goal.Dto {
        [JsonIgnore] public Option<Vector2i> CachedOwnerPosition { get; set; }
        public Vector2i? OwnerPosition {
            get => CachedOwnerPosition.MatchUnsafe<Vector2i?>(x => x, () => null);
            set => CachedOwnerPosition = Optional(value);
        }
        
        public Stack<Node> Path { get; set; }
    }
    
    public ChaseTowerGoal(Brain? brain) : base(brain) { }
    
    public ChaseTowerGoal(Brain? brain, Dto dto) : base(brain, dto) {
        CachedOwnerPosition = dto.CachedOwnerPosition;
        Path = dto.Path;
    }

    private Option<Vector2i> CachedOwnerPosition { get; set; }
    private Stack<Node> Path { get; set; } = new();
    
    public override Option<Thought> NextThought() {
        return Brain.Owner.CurrentRegion.Match<Option<Thought>>(
            region => {
                ValidatePath();
                if (region.Activity.Coords.SquareEuclideanDistance(Brain.Owner.Coords) > Brain.Owner.SquareSightDistance) return None;
                
                switch (Path.Count) {
                    case 0:
                        CachedOwnerPosition = Brain.Owner.Coords;
                        Path = AStar.FindPath(Gamestate.Level.GetNodesInRadius(Brain.Owner.Coords, Brain.Owner.SightDistance), Brain.Owner.Coords, region.Activity.Coords);
                        goto default;
                    default:
                        return GetThought();
                }
            }, None
        );
    }

    private Option<Thought> GetThought() {
        Direction newDirection = Path.Peek().GetDirectionFrom((Vector2i)CachedOwnerPosition!);
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
        if (Path.Count > 0 && !CheckAndUpdateCachedOwnerPosition()) Path.Clear();
    }

    private bool CheckAndUpdateCachedOwnerPosition() {
        if (CachedOwnerPosition.IsNone) return false;

        var ownerCoords = Brain.Owner.Coords;
        if (CachedOwnerPosition == ownerCoords) return true;
        
        CachedOwnerPosition = ownerCoords;
        
        return true;
    }
    
    public override Dto ToDto()
        => new() { Id = Id, CachedOwnerPosition = CachedOwnerPosition, Path = Path, };
}