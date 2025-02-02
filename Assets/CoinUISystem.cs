using UnityEngine;
using TMPro;

public class CoinUISystem : MonoBehaviour
{
    [SerializeField] private GameData gameData;
    [SerializeField] private TMP_Text coinText;

    void Start()
    {
        if (gameData?.playerCore != null)
        {
            UpdateCoinDisplay(gameData.playerCore.Coins);
            gameData.playerCore.onCoinsChanged += UpdateCoinDisplay;
        }
    }

    void OnDestroy()
    {
        if (gameData?.playerCore != null)
            gameData.playerCore.onCoinsChanged -= UpdateCoinDisplay;
    }

    void UpdateCoinDisplay(int amount)
    {
        coinText.text = $"{gameData.playerCore.Coins}";
      //  Debug.Log($"Coins changed by: {amount}");
    }
}