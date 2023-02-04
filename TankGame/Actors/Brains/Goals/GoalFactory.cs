using System;

namespace TankGame.Actors.Brains.Goals; 

public static class GoalFactory {
    public static Goal CreateGoal(GoalType goalType, Brain brain) {
        return goalType  switch {
            GoalType.Idle => new IdleGoal(brain),
            GoalType.BePlayer => new BePlayerGoal(brain),
            GoalType.ChasePlayer => new ChasePlayerGoal(brain),
            GoalType.ChaseTower => new ChaseTowerGoal(brain),
            GoalType.RandomWalk => new RandomWalkGoal(brain),
            GoalType.ShootPlayer => new ShootPlayerGoal(brain),
            GoalType.ShootTower => new ShootTowerGoal(brain),
            _ => throw new ArgumentOutOfRangeException(nameof(goalType), goalType, null),
        };
    }
}