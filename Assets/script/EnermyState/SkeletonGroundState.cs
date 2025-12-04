using UnityEngine;

public class SkeletonGroundState : EnermyState
{
    protected Skeleton skeleton;
    private Transform player => GameObject.Find("Player").transform;

    // 進入戰鬥用的距離門檻（比 Battle 那邊的 keepRange 小一點）
    private const float battleEnterRange = 2f;

    public SkeletonGroundState(Enemy enermy, EnermyStateMachine stateMachine, string animBoolName, Skeleton skeleton)
        : base(enermy, stateMachine, animBoolName)
    {
        this.skeleton = skeleton;
    }

    public override void Update()
    {
        base.Update();

        float distance = Vector2.Distance(player.position, skeleton.transform.position);

        // 玩家夠近 或 射線偵測到 → 進戰鬥
        if (skeleton.IsPlayerDetected() || distance <= battleEnterRange)
        {
            stateMachine.ChangeState(skeleton.battleState);
        }
    }
}
