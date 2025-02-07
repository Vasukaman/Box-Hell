using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class TriggerToActivate : MonoBehaviour
{
    [Tooltip("If checked, any GameObject can activate the trigger")]
    public bool anyTagAllowed = false;

    [Tooltip("Specific tags that can activate the trigger (ignored if Any Tag Allowed is checked)")]
    public List<string> allowedTags = new List<string>();

    [Space]
    public UnityEvent onTriggerActivated;
    public UnityEvent onTriggerDeactivated;

    private Collider triggerCollider;
    [SerializeField] bool destroyAfterActivate;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ShouldTrigger(other.tag))
        {
            onTriggerActivated.Invoke();
            if (destroyAfterActivate) Destroy(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (ShouldTrigger(other.tag))
        {
            onTriggerDeactivated.Invoke();
        }
    }

    private bool ShouldTrigger(string tagToCheck)
    {
        return anyTagAllowed || allowedTags.Contains(tagToCheck);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (triggerCollider == null)
            triggerCollider = GetComponent<Collider>();

        if (!triggerCollider.isTrigger)
        {
            Debug.LogWarning($"Collider on {gameObject.name} must be a trigger! Automatically enabling.", this);
            triggerCollider.isTrigger = true;
        }
    }
#endif
}