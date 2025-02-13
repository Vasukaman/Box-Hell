using UnityEngine;
using TMPro;

public class CoinUISystem : MonoBehaviour
{
    [SerializeField] private PlayerCore playerCore;
    [SerializeField] private TMP_Text coinText;

    void Start()
    {
        if (playerCore != null)
        {
            UpdateCoinDisplay(playerCore.Coins);
            playerCore.onCoinsChanged += UpdateCoinDisplay;
        }
    }

    void OnDestroy()
    {
        if (playerCore != null)
            playerCore.onCoinsChanged -= UpdateCoinDisplay;
    }

    void UpdateCoinDisplay(int amount)
    {
        coinText.text = $"{playerCore.Coins}$";
      //  Debug.Log($"Coins changed by: {amount}");
    }
}