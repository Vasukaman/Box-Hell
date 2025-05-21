using UnityEngine;

public class HealingTool : Tool
{
    [Header("Healing Settings")]
    [SerializeField] private int healAmount = 1;
    [SerializeField] private AudioClip healSound;
    [SerializeField] private ParticleSystem healParticles;
    [SerializeField] private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        // Set up for single use
        maxDurability = 1;
        currentDurability = 1;
        durabilityLossPerUse = 1;
    }

    public override void Use()
    {
        if (isBroken) return;

        base.Use();

        // Get the owner through ItemCore reference
        if (itemCore.owner != null)
        {
            // Apply healing
            itemCore.owner.Heal(healAmount);

            // Play healing effects
            if (healParticles != null)
            {
                healParticles.Play();
            }

            if (audioSource != null && healSound != null)
            {
                audioSource.PlayOneShot(healSound);
            }
        }

        // Destroy the tool after use
        BreakTool();
    }

    protected override void BreakTool()
    {
        // Additional cleanup before destruction
        if (healParticles != null)
        {
            healParticles.transform.parent = null;
            Destroy(healParticles.gameObject, 2f);
        }

        base.BreakTool();
    }
}