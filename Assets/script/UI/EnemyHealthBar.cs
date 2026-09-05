using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// World-space health bar that floats above an enemy and tracks it each frame.
/// Polls the target Entity for its health ratio and self-destructs when the
/// target is gone (e.g. the enemy died and was destroyed).
/// </summary>
public class EnemyHealthBar : MonoBehaviour
{
    public Entity target;
    public Vector3 worldOffset = new Vector3(0f, 1.4f, 0f);
    [SerializeField] private Image fillImage;

    private void LateUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = target.transform.position + worldOffset;
        transform.rotation = Quaternion.identity;

        if (fillImage != null)
        {
            float ratio = target.MaxHealth > 0 ? (float)target.currentHealth / target.MaxHealth : 0f;
            fillImage.fillAmount = ratio;
        }
    }
}
