using UnityEngine;

public class SkeletonBattleState : EnermyState
{
    private Skeleton skeleton;
    private Transform player;

    private float lastAttackTime;

    // ===== AI 可調整參數 =====
    private const float chaseSpeed = 2f;
    private const float retreatSpeed = 1.4f;

    private const float stopDistance = 1.0f;
    private const float retreatDistance = 0.55f;
    private const float retreatBuffer = 0.15f;
    private const float minFlipXDistance = 0.2f;
    // 🔸 這個是「保持戰鬥距離」，要比 GroundState 的 battleEnterRange 大一點
    private const float battleKeepRange = 3f;

    // 🔸 看不到玩家多久才真的退戰
    private const float loseAggroTime = 2f;
    private float loseAggroTimer;

    public SkeletonBattleState(Enemy enemy, EnermyStateMachine stateMachine, string animBoolName, Skeleton sk)
        : base(enemy, stateMachine, animBoolName)
    {
        skeleton = sk;
    }

    public override void Enter()
    {
        base.Enter();

        player = GameObject.Find("Player").transform;

        // 一進戰鬥就給滿仇恨時間
        loseAggroTimer = loseAggroTime;
    }

    
    public override void Update()
    {
        base.Update();

        float distance = Vector2.Distance(player.position, skeleton.transform.position);

        // 1️⃣ 玩家在哪一邊（只負責面向用）
        float xDiff = player.position.x - skeleton.transform.position.x;

        // 預設用目前 facingDir，避免頭頂小晃就鬼畜翻面
        int faceDir = (int)skeleton.facingDir;

        // 只有當水平距離大於一定值才更新面向
        if (Mathf.Abs(xDiff) > minFlipXDistance)
        {
            faceDir = xDiff > 0 ? 1 : -1;
            skeleton.Flip(faceDir > 0);
        }

        // 3️⃣ 移動邏輯
        bool isRetreating = false;

        if (distance < retreatDistance + retreatBuffer)
        {
            // 後退
            skeleton.SetVelocty(-faceDir * retreatSpeed, skeleton.rb.velocity.y);
            isRetreating = true;
        }
        else if (distance > stopDistance)
        {
            // 追擊
            skeleton.SetVelocty(faceDir * chaseSpeed * 2f, skeleton.rb.velocity.y);
        }
        else
        {
            // 停下來
            skeleton.SetVelocty(0, skeleton.rb.velocity.y);
        }

        // 4️⃣ 攻擊邏輯
        var detect = skeleton.IsPlayerDetected();

        // ===== 仇恨維持邏輯 =====
        if (detect || distance <= battleKeepRange)
        {
            loseAggroTimer = loseAggroTime;
        }
        else
        {
            loseAggroTimer -= Time.deltaTime;
        }

        // 攻擊（退後時不打）
        if (detect)
        {
            if (!isRetreating && detect.distance < skeleton.attackDistance)
            {
                if (Time.time >= lastAttackTime)
                {
                    skeleton.SetVelocty(0, skeleton.rb.velocity.y);
                    lastAttackTime = Time.time + 2f;
                    stateMachine.ChangeState(skeleton.attackState);
                }
            }
        }

        // 5️⃣ 脫離戰鬥
        if (loseAggroTimer <= 0f)
        {
            skeleton.SetVelocty(0, skeleton.rb.velocity.y);
            stateMachine.ChangeState(skeleton.idleState);
        }
    }
}

