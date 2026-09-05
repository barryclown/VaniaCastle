using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    // �s�W�@���ܼƨӰO���O�_�w�g���\Ĳ�o����
    private bool isCountering;

    public PlayerCounterAttackState(string animBoolName, Player player, PlayerStateMachine stateMachine) : base(animBoolName, player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // ��l��
        stateTimer = 0.5f; // �o�O�u�\�X�[�աv���ݼĤH�������ɶ�
        isCountering = false;
        player.anim.SetBool("CounterAttackSuccess", false);
    }

    public override void Exit()
    {
        base.Exit();
        // �i����ץ��j���}���A�ɡA�ȥ��N Success �]�^ false�A�_�h�U���Φ^�� Idle �ɷ|�d��
        player.anim.SetBool("CounterAttackSuccess", false);
    }

    public override void Update()
    {
        base.Update();
        player.SetVelocty(0, 0);

        // 反擊已成功，等待反擊動畫播完再回 Idle
        if (isCountering)
        {
            if (triggerCalled)
            {
                stateMachine.ChangeState(player.idleState);
            }
            return;
        }

        // 偵測攻擊範圍內是否有可被反擊（暈眩）的敵人
        Collider2D[] collider = Physics2D.OverlapCircleAll(player.transform.position, player.attackCheckRadius);
        foreach (var collider2d in collider)
        {
            Enemy enemy = collider2d.GetComponent<Enemy>();
            if (enemy != null && enemy.CanBeStunned())
            {
                player.anim.SetBool("CounterAttackSuccess", true);
                isCountering = true;
                triggerCalled = false;

                // 反擊成功造成傷害（反擊高回報＝玩家攻擊力 2 倍）
                enemy.Damage(player.attackDamage * 2, enemy.IsBackAttacked(player.transform.position));
            }
        }

        // 反擊判定窗內沒有成功 → 逾時回 Idle
        if (!isCountering && stateTimer < 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

}