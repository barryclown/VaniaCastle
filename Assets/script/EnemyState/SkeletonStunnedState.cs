using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonStunnedState : EnemyState
{
    private Skeleton skeleton;
    public SkeletonStunnedState(Enemy enemy, EnemyStateMachine stateMachine, string animBoolName, Skeleton _skeleton) : base(enemy, stateMachine, animBoolName)
    {
        skeleton=_skeleton ;
    }

    public override void Enter()
    {
        base.Enter();
        skeleton.fx.InvokeRepeating("RedColorBlink",0,.1f);
        stateTimer=skeleton.stunDuration;
        
        skeleton.SetVelocty(-skeleton.facingDir *skeleton.stunDirection.x, skeleton.stunDirection.y);
    }

    public override void Exit()
    {
        base.Exit();
        skeleton.fx.Invoke("CancelRedBlink", 0);
        skeleton.SetVelocty(0, 0);
        skeleton.canFlip = true;

    }

    public override void Update()
    {
        base.Update();
        if(stateTimer<=0)
            stateMachine.ChangeState(skeleton.idleState);
    }
}
