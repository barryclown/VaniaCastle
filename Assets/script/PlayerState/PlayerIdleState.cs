using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(string animBoolName, Player player, PlayerStateMachine stateMachine) : base(animBoolName, player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocty(0, 0);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (xInput != 0)
        {
            player.stateMachine.ChangeState(player.moveState);
        }
        if (player.rb.velocity.y < 0)
        {
            player.stateMachine.ChangeState(player.airState);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            player.stateMachine.ChangeState(player.dashState);
        }
    }
}
