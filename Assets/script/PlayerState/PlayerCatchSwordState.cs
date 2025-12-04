using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCatchSwordState : PlayerState
{
    private Transform sword;
    public PlayerCatchSwordState(string animBoolName, Player player, PlayerStateMachine stateMachine) : base(animBoolName, player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        sword = player.sword.transform;
        bool swordIsRight = sword.position.x > player.transform.position.x;
        player.Flip(swordIsRight);

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if(triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
}
