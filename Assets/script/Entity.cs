using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Knockback")]
    [SerializeField] protected Vector2 knockBackDirection;
    protected bool isKnocked;

    // 攻擊、特殊動作時用來鎖自動翻面
    public bool canFlip = true;

    [Header("Attack Check")]
    public Transform attackCheck;
    public float attackCheckRadius = 0.5f;

    
    public EntityFX fx { get; private set; }
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    [Header("Environment Check")]
    [SerializeField] protected Transform groundCheck;
    [SerializeField] protected float groundCheckDistance = 0.2f;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance = 0.2f;
    [SerializeField] protected LayerMask whatIsGround;

    [Header("Facing")]
    public float facingDir = 1;
    public bool isFacingRight = true;
    // 依速度自動翻面：玩家要（面向移動方向），敵人不要（靠 state/控制器刻意設面向，
    // 否則後退/被擊退時速度反向會把敵人自動轉成背對玩家＝倒著走）
    protected virtual bool AutoFlipByVelocity => true;

    [Header("Health")]
    [SerializeField] protected int maxHealth = 100;
    [Tooltip("此實體每次被擊中所受到的傷害（舊式：受害者端固定值，沒指定攻擊者傷害時的退路）")]
    [SerializeField] protected int hitDamage = 10;
    [Tooltip("此實體攻擊造成的傷害（攻擊者端，傳給 Damage(int,bool)）")]
    public int attackDamage = 10;
    public int currentHealth { get; protected set; }
    public int MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0;

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        fx = GetComponentInChildren<EntityFX>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    protected virtual void Update()
    {
        FlipController();
    }

    public IEnumerator BusyFor(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    // ===== 判定地板 / 牆壁（Player / Skeleton State 都在用） =====
    public virtual bool IsGroundDetected()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    }

    public virtual bool IsWallDetected()
    {
        return Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    protected virtual void OnDrawGizmos()
    {
        if (groundCheck)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(groundCheck.position,
                new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        }

        if (attackCheck)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius);
        }

        if (wallCheck)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheck.position,
                new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        }
    }

    // ===== 翻面鎖定 API：給 Player / Skeleton 在攻擊時使用 =====
    public void LockFlip()
    {
        canFlip = false;
    }

    public void UnlockFlip()
    {
        canFlip = true;
    }

    // ===== 核心翻面函式（所有翻面都應該走這個） =====
    public void Flip(bool faceRight)
    {
  
        // 被擊退中 or 被鎖翻面時，一律禁止翻面（不管誰呼叫）
        if (isKnocked || !canFlip)
            return;

        // 同方向就不用改（避免抖動）
        if (isFacingRight == faceRight)
            return;

        isFacingRight = faceRight;
        facingDir = faceRight ? 1 : -1;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (faceRight ? 1 : -1);
        transform.localScale = scale;
    }

    // ===== 根據速度自動翻面（只有走路時會生效） =====
    public void FlipController()
    {
        // 敵人不用速度自動翻面（避免後退/擊退時被轉成背對＝倒著走）
        if (!AutoFlipByVelocity)
            return;

        // 被擊退 or 被鎖翻面時，關閉自動翻面
        if (isKnocked || !canFlip)
            return;

        if (rb.velocity.x > 0.01f)
        {
            Flip(true);   // 面向右
            
        }
        else if (rb.velocity.x < -0.01f)
        {
            Flip(false);  // 面向左
    
        }
    }

    // ===== 受傷 / 擊退 =====
    // 統一背刺判定：依 target(this) 相對攻擊者的位置與自身朝向，回傳是否為背面受擊。
    // 取代散落在 PlayerAnimation / SkeletonAnimation / SwordController 的重複表達式。
    public bool IsBackAttacked(Vector3 attackerPosition)
    {
        return (transform.position.x > attackerPosition.x && facingDir > 0)
            || (transform.position.x < attackerPosition.x && facingDir < 0);
    }

    // 主要：由攻擊者指定傷害值
    public virtual void Damage(int damage, bool isBackAttack)
    {
        if (IsDead)
            return;

        ApplyHealthLoss(damage);

        StartCoroutine(HitKnockback(isBackAttack));

        if (IsDead)
            Die();
    }

    // 環境傷害（掉出地圖、陷阱）：只扣血不擊退。
    // 擊退會把剛送回地板的角色又推出去，所以這條路徑刻意不跑 HitKnockback。
    public virtual void DamageWithoutKnockback(int damage)
    {
        if (IsDead)
            return;

        ApplyHealthLoss(damage);

        if (IsDead)
            Die();
    }

    private void ApplyHealthLoss(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
            currentHealth = 0;

        if (fx != null)
            fx.StartCoroutine("FlashWhiteFX");

        if (SfxManager.instance != null)
            SfxManager.instance.PlayHit();
    }

    // 相容：沒指定攻擊者傷害時，退回受害者端 hitDamage
    public virtual void Damage(bool isBackAttack)
    {
        Damage(hitDamage, isBackAttack);
    }

    protected virtual void Die()
    {
    }

    protected virtual IEnumerator HitKnockback(bool isBackAttack)
    {
        isKnocked = true;

        if (isBackAttack)
            rb.velocity = new Vector2(knockBackDirection.x * facingDir, knockBackDirection.y);
        else
            rb.velocity = new Vector2(knockBackDirection.x * -facingDir, knockBackDirection.y);

        // 這段時間內：任何 Flip / FlipController 都會被 isKnocked 擋掉
        yield return new WaitForSeconds(0.1f); // 想要被打退久一點就加長
        rb.velocity= Vector2.zero;
        isKnocked = false;
    }

    // ===== 統一改速度（移動邏輯用這個） =====
    public void SetVelocty(float xVelocty, float yVelocty)
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(xVelocty, yVelocty);
    }
}
