using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    // 敵人不用速度自動翻面，靠各 state / BossController 刻意設面向（修「倒著走」bug）
    protected override bool AutoFlipByVelocity => false;

    protected bool canStunned;
    [SerializeField] protected GameObject counterImage;
    public float stunDuration;
    public Vector2 stunDirection;
    [SerializeField]protected LayerMask whatIsPlayer;
    public EnemyStateMachine stateMachine { get; private set; }
    public float attackDistance = 0.1f;

    protected override void Awake()
    {
        base.Awake();
        stateMachine = new EnemyStateMachine();
    }
    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
    }

    public virtual void FreezeTime(bool timeFrozen)
    {
        if (timeFrozen)
        {
            
            anim.speed = 0;
        }
        else 
        {
            anim.speed = 1;
        }
    }
    protected virtual IEnumerator freezeTimerFor(float second)
    {
        FreezeTime(true);
        yield return new WaitForSeconds(second);
        FreezeTime(false);
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position,new Vector3(transform.position.x+attackDistance*facingDir,transform.position.y));
    }
    
    public virtual RaycastHit2D IsPlayerDetected()=>Physics2D.Raycast(wallCheck.position,Vector2.right*facingDir,50,whatIsPlayer);
    public virtual void FinishTrigger()=>stateMachine.currentState.AnimationFinishTrigger();

    public void OpenCounterImage()
    {
        canStunned = true;
        counterImage.SetActive(true);
    }
    public void CloseCounterImage()
    {
        canStunned = false;
        counterImage.SetActive(false);
    }
    public virtual  bool CanBeStunned()
    {
        if (canStunned)
        {
            CloseCounterImage();
            return true;
        }
         
        return false;
    }

    protected override void Die()
    {
        base.Die();
        Destroy(gameObject);
    }
}



