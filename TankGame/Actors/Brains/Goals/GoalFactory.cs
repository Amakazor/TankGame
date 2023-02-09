using System;

namespace TankGame.Actors.Brains.Goals; 

public static class GoalFactory {
    public static Goal CreateGoal(GoalType goalType, Brain brain)
        => goalType  switch {
            GoalType.Idle        => new IdleGoal(brain),
            GoalType.BePlayer    => new BePlayerGoal(brain),
            GoalType.ChasePlayer => new ChasePlayerGoal(brain),
            GoalType.ChaseTower  => new ChaseTowerGoal(brain),
            GoalType.RandomWalk  => new RandomWalkGoal(brain),
            GoalType.ShootPlayer => new ShootPlayerGoal(brain),
            GoalType.ShootTower  => new ShootTowerGoal(brain),
            _                    => throw new ArgumentOutOfRangeException(nameof(goalType), goalType, null),
        };

    public static Goal CreateGoal(Goal.Dto goalDto, Brain brain)
        => goalDto switch {
            BePlayerGoal.Dto dto    => new BePlayerGoal(brain, dto),
            ChasePlayerGoal.Dto dto => new ChasePlayerGoal(brain, dto),
            ChaseTowerGoal.Dto dto  => new ChaseTowerGoal(brain, dto),
            IdleGoal.Dto dto        => new IdleGoal(brain, dto),
            RandomWalkGoal.Dto dto  => new RandomWalkGoal(brain, dto),
            ShootPlayerGoal.Dto dto => new ShootPlayerGoal(brain, dto),
            ShootTowerGoal.Dto dto  => new ShootTowerGoal(brain, dto),
            _                       => throw new ArgumentOutOfRangeException(nameof(goalDto), goalDto, null)
        };
}