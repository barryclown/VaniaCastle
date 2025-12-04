
using System.Collections;
using UnityEngine;
public class PlayerAirState : PlayerState
{
    public PlayerAirState(string animBoolName, Player player, PlayerStateMachine stateMachine)
        : base(animBoolName, player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (player.IsWallDetected() &&
           !player.IsGroundDetected() &&
           xInput == player.facingDir)
        {
            stateMachine.ChangeState(player.wallSlideState);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.Dash.CheckSkill())
        {

            player.dashDir = Input.GetAxisRaw("Horizontal");

            if (player.dashDir == 0)
                player.dashDir = player.facingDir;
            stateMachine.ChangeState(player.dashState);
        }

        // 水平空中操控
        player.SetVelocty(xInput * 5, player.rb.velocity.y);

        // ====== 1. 落地就回 Idle ======
        if (player.IsGroundDetected())
        {
            player.SetVelocty(0, player.rb.velocity.y);
            stateMachine.ChangeState(player.idleState);
            return;
        }

        // ====== 2. 確定要「滑牆」的條件 ======
        // 需要：有牆 + 沒踩地 + 有往牆推 + 正在往下掉
        if (player.IsWallDetected() &&
            !player.IsGroundDetected() &&
            xInput * player.facingDir > 0)
        {
           
            stateMachine.ChangeState(player.wallSlideState);
            return;
        }
        else
        { }

            // 其他情況就維持空中狀態
        }
}
