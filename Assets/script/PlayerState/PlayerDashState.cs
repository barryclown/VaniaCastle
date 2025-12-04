using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(string animBoolName, Player player, PlayerStateMachine stateMachine) : base(animBoolName, player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();       
        stateTimer = 0.2f;
        SkillManager.instance.Clone.CreatClone();
    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocty(xInput, player.rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();
        
        player.SetVelocty(25f * player.facingDir, 0);
        if (stateTimer < 0)
        {
            player.stateMachine.ChangeState(player.idleState);
        }
    }
   
}
