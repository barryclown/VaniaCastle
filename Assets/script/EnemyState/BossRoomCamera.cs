using UnityEngine;

public class BossRoomCamera : MonoBehaviour
{
    [SerializeField] private MonoBehaviour bossVirtualCamera; // CinemachineVirtualCamera
    [SerializeField] private int activePriority = 30;

    private Collider2D triggerCollider;
    private bool activated;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (activated || bossVirtualCamera == null) return;

        Player player = FindObjectOfType<Player>();
        if (player == null) return;

        Collider2D playerCollider = player.GetComponentInChildren<Collider2D>();
        if (IsPlayerInside(player.transform, playerCollider))
            Activate();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryActivate(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryActivate(other);
    }

    private void TryActivate(Collider2D other)
    {
        if (activated || bossVirtualCamera == null) return;
        bool isPlayer = other.GetComponent<Player>() != null || other.GetComponentInParent<Player>() != null;
        if (!isPlayer) return;

        Activate();
    }

    private bool IsPlayerInside(Transform playerTransform, Collider2D playerCollider)
    {
        if (triggerCollider == null) return false;
        if (playerCollider != null) return triggerCollider.bounds.Intersects(playerCollider.bounds);
        return triggerCollider.bounds.Contains(playerTransform.position);
    }

    private void Activate()
    {
        if (activated) return;
        activated = true;
        SetPriority(activePriority);
    }

    private void SetPriority(int priority)
    {
        var type = bossVirtualCamera.GetType();
        var field = type.GetField("m_Priority");
        if (field != null)
        {
            field.SetValue(bossVirtualCamera, priority);
            return;
        }

        var property = type.GetProperty("Priority");
        if (property != null) property.SetValue(bossVirtualCamera, priority);
    }
}
