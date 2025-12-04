using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonMoveState : SkeletonGroundState
{
    public SkeletonMoveState(Enemy enermy, EnermyStateMachine stateMachine, string animBoolName, Skeleton skeleton)
        : base(enermy, stateMachine, animBoolName, skeleton)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // 持續往前走
        skeleton.SetVelocty(skeleton.facingDir * 1f, skeleton.rb.velocity.y);

        // ---------- 地板 / 牆壁判斷 ----------

        // ① 遇到牆 → 翻面
        if (skeleton.IsWallDetected())
        {
            skeleton.Flip(!skeleton.isFacingRight);
            skeleton.SetVelocty(skeleton.facingDir * 1f, skeleton.rb.velocity.y);
            return;
        }

        // ② 前方沒地板（平台邊緣）→ 翻面
        if (!skeleton.IsGroundDetected())
        {
            skeleton.Flip(!skeleton.isFacingRight);
            skeleton.SetVelocty(skeleton.facingDir * 1f, skeleton.rb.velocity.y);
            return;
        }
    }

}
