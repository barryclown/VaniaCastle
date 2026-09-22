using System;

// StreamingAssets/tuning.json 的資料結構。
// 設計原則：這是「覆寫層」，不是唯一真相來源。
//   - 預設值 = 目前程式碼／Inspector 裡跑得好好的數字（見 CreateDefaults）
//   - JSON 只覆寫它有寫到的欄位；沒寫到的沿用預設，行為與改動前完全一致
//   - 整份 tuning.json 刪掉，遊戲行為 100% 回到 v1.1
// 這樣才不牴觸專案 CLAUDE.md 第六條「靜態數值不寫死、不存 JSON」的用意：
// 靜態權威仍在程式碼／Inspector，JSON 只是給企劃即時試數值的旋鈕。

[Serializable]
public class PlayerTuning
{
    public int maxHealth;
    public int attackDamage;
    public float moveSpeed;
    public float jumpForce;
    public float dashSpeed;
    public float dashDuration;
    public float wallSlideSpeed;
    public float counterWindow;
    public float counterDamageMultiplier;
}

[Serializable]
public class SkeletonTuning
{
    public int maxHealth;
    public int attackDamage;
    public float patrolSpeed;
    public float idleTime;
    public float battleEnterRange;
    public float chaseSpeed;
    public float retreatSpeed;
    public float stopDistance;
    public float retreatDistance;
    public float retreatBuffer;
    public float battleKeepRange;
    public float loseAggroTime;
    public float attackCooldown;
    public float attackDistance;
    public float stunDuration;
}

[Serializable]
public class BossTuning
{
    public int maxHealth;
    public float aggroRange;
    public float chaseSpeed;
    public float meleeRange;
    public float meleeCooldown;
    public int meleeDamage;
    public float counterWindow;
    public float rangedCooldown;
    public float projectileSpeed;
    public int projectileDamage;
    public int maxMinions;
    public float summonCooldown;
    public float enrageHpRatio;
    public float enrageCooldownScale;
    public float enrageSpeedScale;
}

[Serializable]
public class HotReloadTuning
{
    public bool enabled;
    public float pollSeconds;
    public bool showToast;
}

[Serializable]
public class TuningData
{
    public HotReloadTuning hotReload = new HotReloadTuning();
    public PlayerTuning player = new PlayerTuning();
    public SkeletonTuning skeleton = new SkeletonTuning();
    public BossTuning boss = new BossTuning();

    // 「這個欄位沒有值」的哨兵。JsonUtility 不支援 nullable，用哨兵最直白。
    public const float NoFloat = float.NaN;
    public const int NoInt = int.MinValue;

    public static bool Has(float v) { return !float.IsNaN(v); }
    public static bool Has(int v) { return v != NoInt; }

    /// <summary>預設值＝改動前程式碼裡寫死的數字。JSON 沒提供時就用這些。</summary>
    public static TuningData CreateDefaults()
    {
        TuningData d = new TuningData();

        d.hotReload.enabled = true;
        d.hotReload.pollSeconds = 0.5f;
        d.hotReload.showToast = true;

        d.player.maxHealth = NoInt;            // 交給 Inspector，JSON 沒寫就不碰
        d.player.attackDamage = NoInt;
        d.player.moveSpeed = 4f;               // PlayerMoveState
        d.player.jumpForce = NoFloat;          // 交給 Inspector
        d.player.dashSpeed = 25f;              // PlayerDashState
        d.player.dashDuration = 0.2f;          // PlayerDashState
        d.player.wallSlideSpeed = 2.5f;        // PlayerWallSlideState
        d.player.counterWindow = 0.5f;         // PlayerCounterAttackState
        d.player.counterDamageMultiplier = 2f; // PlayerCounterAttackState

        d.skeleton.maxHealth = NoInt;
        d.skeleton.attackDamage = NoInt;
        d.skeleton.patrolSpeed = 1f;           // SkeletonMoveState
        d.skeleton.idleTime = 1f;              // SkeletonIdleState
        d.skeleton.battleEnterRange = 2f;      // SkeletonGroundState
        d.skeleton.chaseSpeed = 4f;            // SkeletonBattleState（原式 2f * 2f）
        d.skeleton.retreatSpeed = 1.4f;
        d.skeleton.stopDistance = 1.0f;
        d.skeleton.retreatDistance = 0.55f;
        d.skeleton.retreatBuffer = 0.15f;
        d.skeleton.battleKeepRange = 3f;
        d.skeleton.loseAggroTime = 2f;
        d.skeleton.attackCooldown = 2f;
        d.skeleton.attackDistance = NoFloat;   // Enemy.attackDistance，交給 Inspector
        d.skeleton.stunDuration = NoFloat;

        // Boss 全部預設交給 Inspector（BossController 本來就有完整預設值），
        // JSON 有寫才覆寫，避免把 v1.1 調好的 Boss 平衡洗掉。
        d.boss.maxHealth = NoInt;
        d.boss.aggroRange = NoFloat;
        d.boss.chaseSpeed = NoFloat;
        d.boss.meleeRange = NoFloat;
        d.boss.meleeCooldown = NoFloat;
        d.boss.meleeDamage = NoInt;
        d.boss.counterWindow = NoFloat;
        d.boss.rangedCooldown = NoFloat;
        d.boss.projectileSpeed = NoFloat;
        d.boss.projectileDamage = NoInt;
        d.boss.maxMinions = NoInt;
        d.boss.summonCooldown = NoFloat;
        d.boss.enrageHpRatio = NoFloat;
        d.boss.enrageCooldownScale = NoFloat;
        d.boss.enrageSpeedScale = NoFloat;

        return d;
    }
}
