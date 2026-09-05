using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 進入遊戲時強制重建 Tilemap 的 Composite 碰撞幾何。
/// 因為程式批次改過的 tile（如競技場地板）有時不會被 CompositeCollider2D 自動納入，
/// 導致角色穿過去；在 Start 重建一次即可確保所有磚都有碰撞。
/// </summary>
[RequireComponent(typeof(TilemapCollider2D))]
public class GroundColliderRebuild : MonoBehaviour
{
    private void Start()
    {
        TilemapColliderRebuilder.Rebuild(GetComponent<Tilemap>());
    }
}
