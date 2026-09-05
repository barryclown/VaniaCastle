using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 玩家掉進 boss 場之後，把「他掉下來的那個入口」用實體磚補起來，讓他回不去。
///
/// 封門區塊放在入口平台的正上方（不是玩家會經過的落點正下方），
/// 所以可以在玩家還在半空中、還沒落地時就補完，不會把磚生在玩家身上；
/// 補完的柱子同時擋掉「從場內起跳回平台」這條逃脫路線。
/// 相機切換交給同物件上的 BossRoomCamera，兩者可各自調整時機。
/// </summary>
public class BossRoomSeal : MonoBehaviour
{
    [Header("封門磚")]
    [SerializeField] private Tilemap sealTilemap;
    [SerializeField] private TileBase sealTile;

    [Header("封門區塊（Tilemap 格座標，含頭含尾）")]
    [SerializeField] private int sealXMin = 41;
    [SerializeField] private int sealXMax = 41;
    [SerializeField] private int sealYMin = -11;
    [SerializeField] private int sealYMax = -8;

    [Header("觸發條件")]
    [Tooltip("玩家要低於這個 y 才封門：確保他已經掉到封門區塊下方，磚不會生在他身上")]
    [SerializeField] private float armBelowY = -12f;

    private Collider2D triggerCollider;
    private Player player;
    private Collider2D playerCollider;
    private bool sealedEntrance;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        CachePlayer();

        if (sealTilemap == null)
        {
            GameObject ground = GameObject.Find("Ground");
            if (ground != null) sealTilemap = ground.GetComponent<Tilemap>();
        }
    }

    private void Update()
    {
        if (sealedEntrance) return;

        if (player == null && !CachePlayer()) return;

        if (IsPlayerInside() && IsPlayerBelowSeal())
            Seal();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TrySeal(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TrySeal(other);
    }

    private bool CachePlayer()
    {
        player = FindObjectOfType<Player>();
        if (player == null) return false;

        playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider == null) playerCollider = player.GetComponentInChildren<Collider2D>();
        return true;
    }

    private void TrySeal(Collider2D other)
    {
        if (sealedEntrance) return;

        Player hit = other.GetComponent<Player>();
        if (hit == null) hit = other.GetComponentInParent<Player>();
        if (hit == null) return;

        if (hit.transform.position.y > armBelowY) return;

        Seal();
    }

    private bool IsPlayerInside()
    {
        if (triggerCollider == null) return false;
        if (playerCollider != null) return triggerCollider.bounds.Intersects(playerCollider.bounds);
        return triggerCollider.bounds.Contains(player.transform.position);
    }

    private bool IsPlayerBelowSeal()
    {
        return player.transform.position.y <= armBelowY;
    }

    public void Seal()
    {
        if (sealedEntrance) return;
        if (sealTilemap == null) return;

        TileBase tile = sealTile != null ? sealTile : FindReusableGroundTile(sealTilemap);
        if (tile == null) return;

        sealedEntrance = true;

        for (int x = sealXMin; x <= sealXMax; x++)
            for (int y = sealYMin; y <= sealYMax; y++)
                sealTilemap.SetTile(new Vector3Int(x, y, 0), tile);

        TilemapColliderRebuilder.Rebuild(sealTilemap);
    }

    private TileBase FindReusableGroundTile(Tilemap tilemap)
    {
        TileBase tile = tilemap.GetTile(new Vector3Int(sealXMin, sealYMin - 1, 0));
        if (tile != null) return tile;

        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile != null) return tile;
            }
        }

        return null;
    }
}
