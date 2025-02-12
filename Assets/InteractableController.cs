using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractableController : MonoBehaviour, IInteractable
{
    [SerializeField] private SelectionFeedback selectionFeedback;
    private Collider _interactionCollider;

    private void Awake()
    {
  
    }

    public void OnSelected()
    {
        selectionFeedback.SetSelected(true);
    }

    public void OnDeselected()
    {
        selectionFeedback.SetSelected(false);
    }
}