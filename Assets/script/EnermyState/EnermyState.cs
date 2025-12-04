using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnermyState 
{
    protected Enemy enemy;
    protected EnermyStateMachine stateMachine;

    protected bool triggerCalled;
    protected string animBoolName;
    protected float stateTimer;

    public EnermyState(Enemy enermy, EnermyStateMachine stateMachine, string animBoolName)
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
