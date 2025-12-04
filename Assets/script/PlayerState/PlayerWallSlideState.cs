using UnityEngine;

public class PlayerWallSlideState : PlayerState
{
    public PlayerWallSlideState(string animBoolName, Player player, PlayerStateMachine stateMachine)
        : base(animBoolName, player, stateMachine)
    {
    }

  
    private const float detachTime = 0.1f; // 離牆緩衝時間

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        bool onGround = player.IsGroundDetected();
        bool onWall = player.IsWallDetected();

        // 1. 落地就回 Idle
        if (onGround)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }
            
        // 3. 從牆上跳開
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.ChangeState(player.wallJumpState);
            return;
        }

       
        if ( xInput==player.facingDir)
            player.SetVelocty(0, -2.5f);
        else
            stateMachine.ChangeState(player.airState);

    }
}
