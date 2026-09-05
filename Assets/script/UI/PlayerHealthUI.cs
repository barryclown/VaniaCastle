using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Top-left player health bar. Polls the player's Entity each frame and
/// drives a Filled Image plus an optional numeric label.
/// </summary>
public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Text label;

    private Entity player;

    private void Start()
    {
        if (PlayerManager.instance != null)
            player = PlayerManager.instance.player;
        if (player == null)
            player = FindObjectOfType<Player>();
    }

    private void LateUpdate()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
            return;
        }

        float ratio = player.MaxHealth > 0 ? (float)player.currentHealth / player.MaxHealth : 0f;
        if (fillImage != null) fillImage.fillAmount = ratio;
        if (label != null) label.text = player.currentHealth + " / " + player.MaxHealth;
    }
}
