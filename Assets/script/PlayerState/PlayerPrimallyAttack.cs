using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimallyAttack : PlayerState
{
    
    public PlayerPrimallyAttack(string animBoolName, Player player, PlayerStateMachine stateMachine) : base(animBoolName, player, stateMachine)
    {
    }
    private int comboCount=0;
    private float lastTimeAttack;
   
    public override void Enter()
    {

        player.transform.position += new Vector3((float)0.2*player.facingDir,0,0) ;
        if (comboCount>2 || lastTimeAttack+2<Time.time) { comboCount = 0; };
        player.anim.SetInteger("AttackCombo",comboCount);
        base.Enter();
        
    }

    public override void Exit()
    {
        triggerCalled = false;
        comboCount++;
        lastTimeAttack=Time.time;
        player.UnlockFlip();
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
      player.SetVelocty((float)0.1 * player.facingDir, 0);
        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.Dash.CheckSkill())
        {

            player.dashDir = Input.GetAxisRaw("Horizontal");

            if (player.dashDir == 0)
                player.dashDir = player.facingDir;
            stateMachine.ChangeState(player.dashState);
        }
    }
}
