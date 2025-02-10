using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHealth : MonoBehaviour
{
    //TEMP
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private PlayerCore player;
    private void Awake()
    {
        player.onHealthChanged += UpdateHealth;
    }

    void UpdateHealth(int newHealth)
    {
        healthText.text = newHealth.ToString();
    }

    private void OnDestroy()
    {
        player.onHealthChanged -= UpdateHealth;
    }
}
