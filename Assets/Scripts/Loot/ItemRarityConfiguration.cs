// ItemRarityConfiguration.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Loot/Item Rarity")]
public class ItemRarityConfiguration : ScriptableObject
{
    public string name;
    public int rank;

    [Header("VFX & SFX")]
    public ParticleSystem dropEffect;
    public ParticleSystem holdEffect;
    public SoundData spawnSound;

    [Header("Gambling Machine UI")]
    [Tooltip("Background tint for this rarity's slot")]
    public Color slotTintColor = Color.white;
    [Tooltip("Icon scale multiplier when highlighting this rarity")]
    public float highlightScale = 1.2f;
    [Tooltip("Animation curve for slot 'pop' on shuffle")]
    public AnimationCurve shuffleCurve = AnimationCurve.EaseInOut(0, 1, 1, 1);
    [SerializeField] public SoundData slotShuffleSound;
    [SerializeField] public SoundData slotPickSound;

}
