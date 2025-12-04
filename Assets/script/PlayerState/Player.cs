using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    public float jumpForce = 6;
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerPrimallyAttack primallyAttack { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }
    public PlayerAimSwordState aimSwordState { get; private set; }
    public PlayerCatchSwordState catchSwordState { get; private set; }
    
    public float dashDir;
    public GameObject sword { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        stateMachine = new PlayerStateMachine();
        moveState=new PlayerMoveState("Move",this,stateMachine);
        idleState = new PlayerIdleState("Idle", this, stateMachine);
        jumpState = new PlayerJumpState("Jump",this,stateMachine);
        airState = new PlayerAirState("Jump",this,stateMachine);
        dashState = new PlayerDashState("Dash", this, stateMachine);
        wallSlideState= new PlayerWallSlideState("WallSlide", this, stateMachine);
        wallJumpState = new PlayerWallJumpState("Jump", this, stateMachine);
        primallyAttack = new PlayerPrimallyAttack("Attack", this, stateMachine);
        counterAttack = new PlayerCounterAttackState("CounterAttack", this,stateMachine);
        aimSwordState = new PlayerAimSwordState("AimSword",this,stateMachine);
        catchSwordState = new PlayerCatchSwordState("CatchSword",this,stateMachine);
    }
    protected override void Start()
    {
        base.Start();
        facingDir = 1;   
        stateMachine.Initialize(idleState);
      
    }
    protected override void Update()
    {
        base .Update();
        stateMachine.currentState.Update();
            
    }
   
    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();
   

   
        
       
    
   public void AssignNewSword(GameObject newSword)
    {
        sword = newSword;
    }
    public void ClearSword()
    {
        stateMachine.ChangeState(catchSwordState);
        Destroy(sword);
    }
    public bool HasNoSword()
    {
        if (sword == null)
            return true;
        else
        {
            sword.GetComponent<SwordController>().ReturnSword();
            return false;
        }
    }
}
