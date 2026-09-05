using System.Reflection;
using UnityEngine;

/// <summary>
/// 掉出地圖的補救：角色掉到 fallThresholdY 以下時扣血，並送回「最後一次站穩的地板」，
/// 而不是無限往下掉。血被扣到 0 時走 Entity 既有的死亡流程（死亡介面）。
/// 掛在 Player 身上即可，不需要在地圖底下鋪一層 kill zone。
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerFallRecovery : MonoBehaviour
{
    [Header("掉落判定")]
    [Tooltip("角色 y 低於此值就視為掉出地圖；要設在地圖最低的地板之下")]
    [SerializeField] private float fallThresholdY = -25f;
    [Tooltip("每次掉出地圖扣多少血")]
    [SerializeField] private int fallDamage = 10;

    [Header("送回地板")]
    [Tooltip("重生時往上墊高一點，避免卡進地板")]
    [SerializeField] private float respawnLift = 0.1f;
    [Tooltip("要站穩多久才記錄成安全點，避免記到剛好踩過去的那一格")]
    [SerializeField] private float groundedSampleTime = 0.15f;

    private Player player;
    private Rigidbody2D body;
    private Vector3 safeGround;
    private float groundedTimer;

    // Cinemachine 用反射呼叫，跟 BossRoomCamera 一樣不對套件做編譯期相依
    private MonoBehaviour[] virtualCameras;
    private MethodInfo warpMethod;
    private readonly object[] warpArgs = new object[2];

    private void Awake()
    {
        player = GetComponent<Player>();
        body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        safeGround = transform.position;
        CacheVirtualCameras();
    }

    private void Update()
    {
        if (player.IsDead) return;

        if (transform.position.y < fallThresholdY)
        {
            Recover();
            return;
        }

        RecordSafeGround();
    }

    /// <summary>
    /// 只在「真的踩穩」時記錄：有地板 + 垂直速度接近 0。
    /// 這樣掉下去的那一瞬間不會把半空中的位置記成安全點。
    /// </summary>
    private void RecordSafeGround()
    {
        if (!player.IsGroundDetected() || Mathf.Abs(body.velocity.y) > 0.1f)
        {
            groundedTimer = 0f;
            return;
        }

        groundedTimer += Time.deltaTime;
        if (groundedTimer >= groundedSampleTime)
            safeGround = transform.position;
    }

    private void Recover()
    {
        Vector3 from = transform.position;
        Vector3 to = safeGround + new Vector3(0f, respawnLift, 0f);

        transform.position = to;
        body.velocity = Vector2.zero;
        groundedTimer = 0f;

        // 先送回地板再扣血：萬一這一下扣死了，死亡畫面停在地板上而不是虛空
        WarpCameras(to - from);
        player.DamageWithoutKnockback(fallDamage);
    }

    private void CacheVirtualCameras()
    {
        MonoBehaviour[] all = FindObjectsOfType<MonoBehaviour>();
        int count = 0;
        for (int i = 0; i < all.Length; i++)
            if (all[i].GetType().Name == "CinemachineVirtualCamera") count++;

        virtualCameras = new MonoBehaviour[count];
        int next = 0;
        for (int i = 0; i < all.Length; i++)
        {
            if (all[i].GetType().Name != "CinemachineVirtualCamera") continue;
            virtualCameras[next++] = all[i];
            if (warpMethod == null)
                warpMethod = all[i].GetType().GetMethod("OnTargetObjectWarped",
                    new System.Type[] { typeof(Transform), typeof(Vector3) });
        }
    }

    /// <summary>
    /// 告訴鏡頭「這是瞬移不是移動」，否則 Cinemachine 會從虛空一路平滑追回來。
    /// </summary>
    private void WarpCameras(Vector3 delta)
    {
        if (virtualCameras == null || warpMethod == null) return;

        warpArgs[0] = transform;
        warpArgs[1] = delta;

        for (int i = 0; i < virtualCameras.Length; i++)
        {
            if (virtualCameras[i] == null) continue;
            warpMethod.Invoke(virtualCameras[i], warpArgs);
        }
    }
}
