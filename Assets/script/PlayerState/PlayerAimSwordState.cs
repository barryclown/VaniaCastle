using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    private Camera cam;

    public PlayerAimSwordState(string animBoolName, Player player, PlayerStateMachine stateMachine) : base(animBoolName, player, stateMachine)
    {
    }

    public override void Enter()
    {
        cam = Camera.main;
        SkillManager.instance.Sword.SetSwordGravity();
        base.Enter();
    }

    public override void Exit()
    {
        SkillManager.instance.Sword.DotsActive(false);
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        SkillManager.instance.Sword.finalDir = new Vector2(SkillManager.instance.Sword.AimDirection().normalized.x * SkillManager.instance.Sword.lanchForce.x, SkillManager.instance.Sword.AimDirection().normalized.y * SkillManager.instance.Sword.lanchForce.y);
        for (int i = 0; i < SkillManager.instance.Sword.dots.Length; i++)
        {
            SkillManager.instance.Sword.dots[i].transform.position = SkillManager.instance.Sword.DotsPosition(i * SkillManager.instance.Sword.spaceBetweenDots);
            SkillManager.instance.Sword.DotsActive(true);
        }
        player.SetVelocty(0,0);
        if(Input.GetKeyUp(KeyCode.Mouse1))
            stateMachine.ChangeState(player.idleState);

        if (cam == null) cam = Camera.main;
        Vector2 mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);
        bool mouseIsRight = mousePosition.x < player.transform.position.x;
        player.Flip(mouseIsRight);
    }
}
