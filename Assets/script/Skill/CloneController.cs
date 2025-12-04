using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneController : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sr;
    private float ColorLosingSpeed=1;
    private float Timer;
    private float Duration=1.5f;
    public float attackCheckRadius;
    public GameObject attackCheck;
    private Collider2D closestEnemy =null;
    private void Awake()
    {
        Timer=Duration;
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        Timer -= Time.deltaTime;
        if(Timer <= 0)
            sr.color=new Color(1f,1f,1f,sr.color.a-Time.deltaTime*ColorLosingSpeed);
        if (SkillManager.instance.Clone.canAttack)
            anim.SetInteger("AttackNumber", Random.Range(0,4));
        if (sr.color.a <= 0)
            Destroy(gameObject);
    }
    private void AttackStart()
    {
        
    }

    // 🔽 新增：在攻擊最後一幀加 AnimationEvent 呼叫這個
    private void AttackEnd()
    {
       
    }
    private void AnimationTrigger()
    {
       Timer = 0;
    }
    private void AttackTrigger()
    {
        CheckTarget();
        Collider2D[] collider = Physics2D.OverlapCircleAll(attackCheck.transform.position, attackCheckRadius);
        foreach (var collider2d in collider)
        {
            if (collider2d.GetComponent<Enemy>() != null)
            {
                Enemy enemy = collider2d.GetComponent<Enemy>();
                if (enemy.transform.position.x > transform.position.x && enemy.facingDir > 0 || enemy.transform.position.x < transform.position.x && enemy.facingDir < 0)
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
    private void CheckTarget()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 25);
        foreach (var collider2d in collider)
        {
            if (collider2d.GetComponent<Enemy>() && closestEnemy == null)
            {
                closestEnemy = collider2d;            
            }
            if (collider2d.GetComponent<Enemy>())
            {
                if (Vector3.Distance(transform.position, collider2d.transform.position) < Vector3.Distance(transform.position, closestEnemy.transform.position))
                    closestEnemy = collider2d;

            }
        }
        if (closestEnemy!=null&&closestEnemy.transform.position.x < transform.position.x)
            transform.Rotate(0, 180, 0);
    }
}
