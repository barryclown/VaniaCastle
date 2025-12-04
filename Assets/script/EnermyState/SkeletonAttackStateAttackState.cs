using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAttackState : EnermyState
{
   
    private Skeleton Skeleton;
    public SkeletonAttackState(Enemy enermy, EnermyStateMachine stateMachine, string animBoolName, Skeleton skeleton) : base(enermy, stateMachine, animBoolName)
    {
        Skeleton = skeleton;
    }

    public override void Enter()
    {
        Skeleton.SetVelocty(0,0);

        base.Enter();
                     
    }

    public override void Exit()
    {
        base.Exit();
      
    }

    public override void Update()
    {
        base.Update();
        Skeleton.SetVelocty(0, 0);
        if (triggerCalled)
         stateMachine.ChangeState(Skeleton.battleState);
         
        
            
    }
}
