using UnityEngine;

/// <summary>
/// Boss 遠程攻擊彈：朝設定方向直線飛行，碰到玩家造成傷害後消失，逾時自毀。
/// 由 BossController 在程式中動態生成（自帶 trigger collider），不依賴動畫事件。
/// </summary>
public class BossProjectile : MonoBehaviour
{
    public Vector2 velocity;
    public float life = 5f;
    public int damage = 12;

    private void Start()
    {
        Destroy(gameObject, life);
    }

    private void Update()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player pl = other.GetComponent<Player>();
        if (pl == null) pl = other.GetComponentInParent<Player>();
        if (pl != null)
        {
            pl.Damage(damage, false);
            Destroy(gameObject);
        }
    }
}
