using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAttackState : EnemyState
{
   
    private Skeleton Skeleton;
    public SkeletonAttackState(Enemy enermy, EnemyStateMachine stateMachine, string animBoolName, Skeleton skeleton) : base(enermy, stateMachine, animBoolName)
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
        // 離開攻擊 state 一律解鎖翻面：避免攻擊被打斷時 AttackEnd event 沒觸發導致 canFlip 卡在 false
        Skeleton.UnlockFlip();
    }

    public override void Update()
    {
        base.Update();
        Skeleton.SetVelocty(0, 0);
        if (triggerCalled)
         stateMachine.ChangeState(Skeleton.battleState);
         
        
            
    }
}
