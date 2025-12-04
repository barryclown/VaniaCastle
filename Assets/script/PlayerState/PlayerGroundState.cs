using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundState : PlayerState
{
    public PlayerGroundState(string animBoolName, Player player, PlayerStateMachine stateMachine) : base(animBoolName, player, stateMachine)
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
        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.Dash.CheckSkill())
        {

            player.dashDir = Input.GetAxisRaw("Horizontal");

            if (player.dashDir == 0)
                player.dashDir = player.facingDir;
            stateMachine.ChangeState(player.dashState);
        }
        if (Input.GetKeyDown(KeyCode.E) && player.HasNoSword())
        {
            player.fx.StartCoroutine("FlashYellowFX");
            SkillManager.instance.Sword.SwitchSwordType();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1)&&player.HasNoSword()) 
        {
            player.stateMachine.ChangeState(player.aimSwordState);
        }
        if (!player.IsGroundDetected()) 
        { 
            player.stateMachine.ChangeState(player.airState);
        }
        if (Input.GetKeyDown(KeyCode.Space)&&player.IsGroundDetected())
        {
            player.stateMachine.ChangeState(player.jumpState);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            player.stateMachine.ChangeState(player.primallyAttack);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            player.stateMachine.ChangeState(player.counterAttack);
        }
    }
}
