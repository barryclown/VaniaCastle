using UnityEngine;

/// <summary>
/// Lightweight horizontal separation for simple melee enemies so identical AI does not stay perfectly stacked.
/// </summary>
[DisallowMultipleComponent]
public class EnemySeparation2D : MonoBehaviour
{
    [SerializeField] private float radius = 1.2f;
    [SerializeField] private float pushStrength = 2.2f;
    [SerializeField] private float maxStepPerFrame = 0.08f;

    private Rigidbody2D rb;
    private static readonly Collider2D[] hits = new Collider2D[12];

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (radius <= 0f || pushStrength <= 0f) return;

        int count = Physics2D.OverlapCircleNonAlloc(transform.position, radius, hits);
        float push = 0f;

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = hits[i];
            if (hit == null || hit.transform == transform) continue;

            Enemy other = hit.GetComponentInParent<Enemy>();
            if (other == null || other.gameObject == gameObject) continue;
            if (other.GetComponent<BossController>() != null) continue;

            float dx = transform.position.x - other.transform.position.x;
            if (Mathf.Abs(dx) < 0.05f)
                dx = GetInstanceID() > other.GetInstanceID() ? 1f : -1f;

            float distance = Mathf.Max(0.05f, Mathf.Abs(dx));
            float weight = 1f - Mathf.Clamp01(distance / radius);
            push += Mathf.Sign(dx) * weight;
        }

        if (Mathf.Abs(push) < 0.01f) return;

        float step = Mathf.Clamp(push * pushStrength * Time.fixedDeltaTime, -maxStepPerFrame, maxStepPerFrame);
        if (rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
            rb.MovePosition(rb.position + Vector2.right * step);
        else
            transform.position += new Vector3(step, 0f, 0f);
    }
}
