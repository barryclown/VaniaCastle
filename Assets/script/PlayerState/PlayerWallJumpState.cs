using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallJumpState :PlayerState
{
    public PlayerWallJumpState(string animBoolName, Player player, PlayerStateMachine stateMachine) : base(animBoolName, player, stateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocty(player.facingDir * -5, player.jumpForce);
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
        if (xInput!=0) 
        player.SetVelocty(xInput * 5, player.rb.velocity.y);
        if (player.rb.velocity.y < 0)
        {
            player.stateMachine.ChangeState(player.airState);
        }
    }
}
