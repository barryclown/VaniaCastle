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
            if (collider2d.GetComponent<Enemy>() != null)
            {
                Enemy enemy = collider2d.GetComponent<Enemy>();
                if (enemy.transform.position.x>transform.position.x&& enemy.facingDir>0|| enemy.transform.position.x < transform.position.x && enemy.facingDir < 0)
                {
                    enemy.Damage(true);
                }
                else
                {
                    enemy.Damage(false);
                }
               
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
