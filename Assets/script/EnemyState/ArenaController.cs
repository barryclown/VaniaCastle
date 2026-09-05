using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 死鬥場單向封門：玩家踏入入口觸發區後，用實體 tilemap 牆磚把左側入口補滿封死，
/// 讓玩家進得來、出不去（右牆與地面為靜態實牆）。
/// 牆磚自動取自門柱下方的地面磚；封門後重建 Composite 碰撞讓新牆即時生效。
/// </summary>
public class ArenaController : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private int gateX = -7;
    [SerializeField] private int gateYMin = -2;
    [SerializeField] private int gateYMax = 6;

    private bool sealedGate;

    private void Awake()
    {
        if (wallTile == null && groundTilemap != null)
            wallTile = groundTilemap.GetTile(new Vector3Int(gateX, gateYMin - 1, 0)); // 門柱下方地面磚
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (sealedGate) return;
        Player pl = other.GetComponent<Player>();
        if (pl == null) pl = other.GetComponentInParent<Player>();
        if (pl == null) return;
        // 只有玩家確實進入場內（門的右側）才封門，避免被擊退到門外時把自己鎖在外面
        if (pl.transform.position.x > gateX + 1f)
            SealGate();
    }

    public void SealGate()
    {
        if (sealedGate || groundTilemap == null || wallTile == null) return;
        sealedGate = true;

        for (int y = gateYMin; y <= gateYMax; y++)
            groundTilemap.SetTile(new Vector3Int(gateX, y, 0), wallTile);

        // 讓 runtime 新增的牆磚真的產生碰撞（只 toggle enabled 是無效的，見 TilemapColliderRebuilder）
        TilemapColliderRebuilder.Rebuild(groundTilemap);
    }
}
