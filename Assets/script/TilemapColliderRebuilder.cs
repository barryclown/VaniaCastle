using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Runtime 改過 tile 之後，把 Tilemap 的碰撞幾何真正重建出來。
///
/// 為什麼需要這支：對「掛在 CompositeCollider2D 底下的 TilemapCollider2D」，
/// 剛 SetTile 完直接呼叫 GenerateGeometry() 拿到的是舊形狀——
/// 舊寫法 tc.enabled = false / true 也一樣無效（實測封門磚畫得出來但 OverlapPoint 完全打不到），
/// 結果就是「看得到、穿得過去」的假牆。
/// 真正有效的是把 usedByComposite 退掛再重掛，強迫 composite 重新收一次形狀。
/// </summary>
public static class TilemapColliderRebuilder
{
    public static void Rebuild(Tilemap tilemap)
    {
        if (tilemap == null) return;

        tilemap.RefreshAllTiles();

        TilemapCollider2D tilemapCollider = tilemap.GetComponent<TilemapCollider2D>();
        if (tilemapCollider == null) return;

        // 先把這一幀剛改的 tile 同步進 collider，不要等它自己延遲處理（2022.2+）
        tilemapCollider.ProcessTilemapChanges();

        CompositeCollider2D composite = tilemapCollider.GetComponent<CompositeCollider2D>();
        if (composite == null)
        {
            // 沒有 composite 就只要 collider 自己更新即可
            tilemapCollider.enabled = false;
            tilemapCollider.enabled = true;
            return;
        }

        // 退掛再重掛：composite 只有在成員重新註冊時才會吃到新的 tile 形狀
        tilemapCollider.usedByComposite = false;
        tilemapCollider.usedByComposite = true;
        composite.GenerateGeometry();
    }
}
