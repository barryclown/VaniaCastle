using UnityEngine;

public class SkeletonGroundState : EnemyState
{
    protected Skeleton skeleton;
    private Transform player => GameObject.Find("Player").transform;

    // �i�J�԰��Ϊ��Z�����e�]�� Battle ���䪺 keepRange �p�@�I�^
    private const float battleEnterRange = 2f;

    public SkeletonGroundState(Enemy enermy, EnemyStateMachine stateMachine, string animBoolName, Skeleton skeleton)
        : base(enermy, stateMachine, animBoolName)
    {
        this.skeleton = skeleton;
    }

    public override void Update()
    {
        base.Update();

        float distance = Vector2.Distance(player.position, skeleton.transform.position);

        // ���a���� �� �g�u������ �� �i�԰�
        if (skeleton.IsPlayerDetected() || distance <= battleEnterRange)
        {
            stateMachine.ChangeState(skeleton.battleState);
        }
    }
}
