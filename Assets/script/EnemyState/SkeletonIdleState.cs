using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonIdleState : SkeletonGroundState
{
    public SkeletonIdleState(Enemy enermy, EnemyStateMachine stateMachine, string animBoolName, Skeleton skeleton)
        : base(enermy, stateMachine, animBoolName, skeleton)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        stateTimer = 1f; // 停留 Idle 時間
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // 偵測牆壁時翻面，避免卡住 Idle
        if (skeleton.IsWallDetected())
        {
            skeleton.Flip(!skeleton.isFacingRight);
        }

        if (stateTimer <= 0 && skeleton.IsGroundDetected())
        {
            stateMachine.ChangeState(skeleton.moveState);
        }
    }

}
