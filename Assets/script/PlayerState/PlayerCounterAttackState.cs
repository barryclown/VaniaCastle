using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    // 新增一個變數來記錄是否已經成功觸發反擊
    private bool isCountering;

    public PlayerCounterAttackState(string animBoolName, Player player, PlayerStateMachine stateMachine) : base(animBoolName, player, stateMachine)
    {
    }

    public override void Enter()
    {
        Debug.Log("Enter CounterAttack State");
        base.Enter();
        // 初始化
        stateTimer = 0.5f; // 這是「擺出架勢」等待敵人攻擊的時間
        isCountering = false;
        player.anim.SetBool("CounterAttackSuccess", false);
    }

    public override void Exit()
    {
        base.Exit();
        // 【關鍵修正】離開狀態時，務必將 Success 設回 false，否則下次或回到 Idle 時會卡住
        player.anim.SetBool("CounterAttackSuccess", false);
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocty(0, 0);

        // 若反擊成功，等待動畫結束
        if (isCountering)
        {
            if (triggerCalled)
            {
                stateMachine.ChangeState(player.idleState);
            }
            return;
        }

        // 檢測是否反擊成功
        Collider2D[] collider = Physics2D.OverlapCircleAll(player.transform.position, player.attackCheckRadius);
        foreach (var collider2d in collider)
        {
            Enemy enemy = collider2d.GetComponent<Enemy>();
            if (enemy != null && enemy.CanBeStunned())
            {
                player.anim.SetBool("CounterAttackSuccess", true);
                isCountering = true;
                triggerCalled = false;
            }
        }

        // 擺出架勢但沒反擊成功 → Timeout
        if (!isCountering && stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

}