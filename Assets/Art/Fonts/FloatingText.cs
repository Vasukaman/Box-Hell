using UnityEngine;
using TMPro;
using DG.Tweening;

    
public class FloatingText : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveHeight = 2f;
    [SerializeField] private float moveDuration = 1.5f;
    [SerializeField] private Ease moveEase = Ease.OutQuad;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Ease fadeEase = Ease.InQuad;

    [SerializeField] private TMP_Text textMesh;
    private Transform mainCamera;
    private Vector3 originalPosition;

    private void Awake()
    {
        mainCamera = Camera.main.transform;
     
    }

    public void FloatText(string message, Color color)
    {
        // Reset position and appearance
     
        // transform.position += 0.5f*Vector3.up;
        textMesh.text = message;
        textMesh.color = color;
        textMesh.alpha = 1f;
        gameObject.SetActive(true);
        originalPosition = transform.position + Vector3.up * 0.5f;
        transform.parent.SetParent(null, true);
        transform.position = originalPosition;
        // Create movement sequence
        Sequence floatSequence = DOTween.Sequence();

        // Move upward
        floatSequence.Append(transform.DOMoveY(originalPosition.y + moveHeight, moveDuration))
            .SetEase(moveEase);

        // Fade out
        floatSequence.Join(textMesh.DOFade(0f, fadeDuration))
            .SetEase(fadeEase);

        // Clean up after animation
        floatSequence.OnComplete(() => Destroy(gameObject));
    }

    public void ReparentHigher()
    {
        transform.SetParent(transform.parent.parent);
    }

    private void Update()
    {
        // Make text always face the camera (billboarding)
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.forward);
        }
    }
}