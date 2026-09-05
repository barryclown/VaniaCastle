using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState 
{
    protected Enemy enemy;
    protected EnemyStateMachine stateMachine;

    protected bool triggerCalled;
    protected string animBoolName;
    protected float stateTimer;

    public EnemyState(Enemy enermy, EnemyStateMachine stateMachine, string animBoolName)
    {
        this.enemy = enermy;
        this.stateMachine = stateMachine;
        
        this.animBoolName = animBoolName;
    }
    public virtual void Update()
    {
        stateTimer-= Time.deltaTime;
    }
    public virtual void Enter()
    {
        triggerCalled = false;
        enemy.anim.SetBool(animBoolName,true);
    }
    public virtual void Exit()
    {
        enemy.anim.SetBool(animBoolName, false);
    }
    public virtual void AnimationFinishTrigger()
    {
        triggerCalled= true;
    }
}
