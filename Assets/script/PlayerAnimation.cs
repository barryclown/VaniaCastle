using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerAnimation : MonoBehaviour
{
    public Player player;
    private void AnimationTrigger()
    {
        player.AnimationTrigger();   
    }
    private void AttackTrigger()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(player.attackCheck.transform.position,player.attackCheckRadius);
        foreach (var collider2d in collider)
        {
            Enemy enemy = collider2d.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Damage(player.attackDamage, enemy.IsBackAttacked(transform.position));
            }
        }
    }
    private void AttackStart()
    {
        player.LockFlip();
    }

    // 🔽 新增：在攻擊最後一幀加 AnimationEvent 呼叫這個
    private void AttackEnd()
    {
        player.UnlockFlip();
    }
    private void ThrowSword()
    {
        SkillManager.instance.Sword.CreatSword();
    }
}
