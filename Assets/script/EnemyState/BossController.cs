using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Boss 戰鬥驅動（自給自足、純程式，不依賴 Animator 狀態轉換或動畫事件，確保可靠）。
/// 與既有 Skeleton/Enemy 共存：Skeleton AI 留作動畫表現，本控制器負責
/// 「鎖定面向＋追擊＋保證傷害＋特色技能（召喚 / 遠程彈 / 狂暴）」。
/// 移動放 FixedUpdate（蓋過 Skeleton AI 設的速度），其餘放 Update。
/// </summary>
[RequireComponent(typeof(Enemy))]
public class BossController : MonoBehaviour
{
    [Header("Aggro / 移動")]
    [Tooltip("在此範圍內一律鎖定玩家（死鬥場通常設很大）")]
    public float aggroRange = 30f;
    public float chaseSpeed = 3.2f;

    [Header("近戰（含反擊預告窗）")]
    public float meleeRange = 3.2f;
    public float meleeCooldown = 1.4f;
    public int meleeDamage = 18;
    private float meleeTimer;
    [Tooltip("近戰前的反擊預告窗：這段時間內玩家用 Q 反擊可把 boss 打暈、取消攻擊")]
    public float counterWindow = 0.6f;
    private float windupTimer;
    private bool windingUp;

    [Header("遠程彈")]
    public float rangedCooldown = 3.2f;
    public float projectileSpeed = 9f;
    public int projectileDamage = 12;

    private float rangedTimer;

    [Header("召喚")]
    public GameObject minionPrefab;
    public int maxMinions = 2;
    public float summonCooldown = 8f;
    private float summonTimer;
    private readonly List<GameObject> minions = new List<GameObject>();

    [Header("狂暴")]
    [Range(0f, 1f)] public float enrageHpRatio = 0.35f;
    public float enrageCooldownScale = 0.6f;
    public float enrageSpeedScale = 1.4f;
    private bool enraged;

    [Header("競技場夾位（防玩家把 boss 釣出場外）")]
    public bool clampToArena = true;
    public float arenaMinX = -6f;
    public float arenaMaxX = 7f;

    private Enemy boss;
    private Skeleton skel;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Transform player;
    private Player playerEntity;

    private void Awake()
    {
        boss = GetComponent<Enemy>();
        skel = GetComponent<Skeleton>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    // boss 是否正被玩家反擊暈眩中
    private bool IsStunned()
    {
        return skel != null && skel.stateMachine != null && skel.stateMachine.currentState == skel.stunnedState;
    }

    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        if (playerEntity != null) player = playerEntity.transform;
        meleeTimer = Random.Range(meleeCooldown * 0.55f, meleeCooldown);
        rangedTimer = Random.Range(rangedCooldown * 0.45f, rangedCooldown);
        summonTimer = Random.Range(summonCooldown * 0.65f, summonCooldown);
    }

    private void Update()
    {
        if (player == null || boss == null || boss.IsDead) return;

        float dx = player.position.x - transform.position.x;
        float dist = Vector2.Distance(player.position, transform.position);
        if (dist > aggroRange) return;

        // 被玩家反擊暈眩中：取消攻擊預告、暫停動作，讓暈眩演完
        if (IsStunned())
        {
            if (windingUp) { windingUp = false; boss.CloseCounterImage(); meleeTimer = meleeCooldown; }
            return;
        }

        // 永遠面向玩家（死鬥場鎖定）
        if (Mathf.Abs(dx) > 0.2f) boss.Flip(dx > 0);

        // 狂暴
        if (!enraged && boss.currentHealth <= boss.MaxHealth * enrageHpRatio) Enrage();
        float cd = enraged ? enrageCooldownScale : 1f;

        // 近戰：先開「反擊預告窗」，窗內玩家可 Q 反擊把 boss 打暈；沒被反擊才造成傷害
        meleeTimer -= Time.deltaTime;
        if (!windingUp)
        {
            if (dist <= meleeRange && meleeTimer <= 0f)
            {
                boss.OpenCounterImage();   // canStunned=true + 顯示反擊圖示，玩家可在這段時間反擊
                windupTimer = counterWindow;
                windingUp = true;
            }
        }
        else
        {
            windupTimer -= Time.deltaTime;
            if (windupTimer <= 0f)
            {
                // 沒被反擊 → 收窗 + 造成傷害
                boss.CloseCounterImage();
                if (dist <= meleeRange && playerEntity != null && !playerEntity.IsDead)
                    playerEntity.Damage(meleeDamage, playerEntity.IsBackAttacked(transform.position));
                windingUp = false;
                meleeTimer = meleeCooldown * cd;
            }
        }

        // 遠程彈：拉開距離時射擊
        rangedTimer -= Time.deltaTime;
        if (dist > meleeRange * 0.6f && rangedTimer <= 0f)
        {
            FireProjectile(dx >= 0f ? 1f : -1f);
            rangedTimer = rangedCooldown * cd;
        }

        // 召喚小兵（有上限）
        summonTimer -= Time.deltaTime;
        minions.RemoveAll(m => m == null);
        if (summonTimer <= 0f && minions.Count < maxMinions && minionPrefab != null)
        {
            Summon();
            summonTimer = summonCooldown * cd;
        }
    }

    private void FixedUpdate()
    {
        if (player == null || boss == null || boss.IsDead || rb == null) return;
        // 暈眩中不主動移動（讓擊退/暈眩演完）；攻擊預告窗站定讓玩家讀招
        if (IsStunned()) return;
        if (windingUp) { rb.velocity = new Vector2(0f, rb.velocity.y); return; }
        float dx = player.position.x - transform.position.x;
        float dist = Vector2.Distance(player.position, transform.position);
        float speed = chaseSpeed * (enraged ? enrageSpeedScale : 1f);

        // 追到近戰距離就停（保留 y 速度給重力）
        if (dist > aggroRange)
            return;
        if (Mathf.Abs(dx) > meleeRange * 0.85f)
            rb.velocity = new Vector2(Mathf.Sign(dx) * speed, rb.velocity.y);
        else
            rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    private void LateUpdate()
    {
        if (!clampToArena || boss == null) return;
        var pos = transform.position;
        float cx = Mathf.Clamp(pos.x, arenaMinX, arenaMaxX);
        if (!Mathf.Approximately(cx, pos.x))
        {
            transform.position = new Vector3(cx, pos.y, pos.z);
            if (rb != null) rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    private void Enrage()
    {
        enraged = true;
        if (sr != null) sr.color = new Color(1f, 0.45f, 0.45f, 1f);
    }

    private void FireProjectile(float dir)
    {
        var go = new GameObject("BossProjectile");
        go.transform.position = transform.position + new Vector3(dir * 1.2f, 0.2f, 0f);

        var psr = go.AddComponent<SpriteRenderer>();
        psr.sprite = GetProjectileSprite();
        psr.color = enraged ? new Color(1f, 0.4f, 0.2f, 1f) : new Color(0.7f, 0.5f, 1f, 1f);
        if (sr != null) psr.sortingLayerID = sr.sortingLayerID;
        psr.sortingOrder = 50;
        go.transform.localScale = Vector3.one * 0.6f;

        var rbp = go.AddComponent<Rigidbody2D>();
        rbp.bodyType = RigidbodyType2D.Kinematic;
        rbp.gravityScale = 0f;

        var cc = go.AddComponent<CircleCollider2D>();
        cc.isTrigger = true;
        cc.radius = 0.5f;

        var proj = go.AddComponent<BossProjectile>();
        proj.velocity = new Vector2(dir * projectileSpeed, 0f);
        proj.life = 5f;
        proj.damage = projectileDamage;
    }

    private static Sprite cachedProjSprite;
    private static Sprite GetProjectileSprite()
    {
        if (cachedProjSprite != null) return cachedProjSprite;
        int size = 16;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 c = new Vector2(size / 2f, size / 2f);
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), c);
                tex.SetPixel(x, y, d <= size / 2f ? Color.white : new Color(1f, 1f, 1f, 0f));
            }
        tex.Apply();
        cachedProjSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        return cachedProjSprite;
    }

    private void Summon()
    {
        var m = Instantiate(minionPrefab);
        m.name = "Boss_Minion";
        m.SetActive(true);
        float side = minions.Count % 2 == 0 ? -1f : 1f;
        float spacing = 2f + Random.Range(0f, 1.25f);
        m.transform.position = transform.position + new Vector3(side * spacing, 1.5f, 0f);
        m.transform.localScale = Vector3.one; // 小兵維持正常大小
        if (m.GetComponent<EnemySeparation2D>() == null)
            m.AddComponent<EnemySeparation2D>();
        minions.Add(m);
    }
}
