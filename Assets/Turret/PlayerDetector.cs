using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    public event System.Action<Transform> OnPlayerDetected;
    public event System.Action OnPlayerLost;

    [Header("Filtering")]
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private string _playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidPlayer(other)) return;
        OnPlayerDetected?.Invoke(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsValidPlayer(other)) return;
        OnPlayerLost?.Invoke();
    }

    private bool IsValidPlayer(Component other)
    { PlayerCore playerCore = other.GetComponent<PlayerCore>();
        return (playerCore != null);
    }

}
