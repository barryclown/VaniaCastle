using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAnimation : MonoBehaviour
{
    private Skeleton skeleton;

    private void Awake()
    {
        skeleton = GetComponentInParent<Skeleton>();
    }

    private void AnimatonTrigger()
    {
        skeleton.FinishTrigger();
    }
    private void AttackTrigger()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(skeleton.transform.position, skeleton.attackCheckRadius);
        foreach (var collider2d in collider)
        {
            Player enermy = collider2d.GetComponent<Player>();
            if (enermy != null)
            {
                enermy.Damage(skeleton.attackDamage, enermy.IsBackAttacked(transform.position));
            }
        }
    }
    private void Open()=>skeleton.OpenCounterImage();
    private void Close() => skeleton.CloseCounterImage();
    private void AttackStart()
    {
        skeleton.LockFlip();
    }

    // 🔽 新增：在攻擊最後一幀加 AnimationEvent 呼叫這個
    private void AttackEnd()
    {
        skeleton.UnlockFlip();
    }
}
