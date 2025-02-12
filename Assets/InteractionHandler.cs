using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    [SerializeField] private float _interactionRange = 5f;
    [SerializeField] private LayerMask _interactableLayer;

    private Camera _mainCamera;
    private IInteractable _currentInteractable;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        HandleRaycast();
    }

    private void HandleRaycast()
    {
        Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, _interactionRange, _interactableLayer))
        { ClearSelection();
            return; }

        var newInteractable = hit.collider.GetComponent<IInteractable>();

        if (newInteractable!=null)
        {
            HandleHit(hit.collider);
        }
        else
        {
            ClearSelection();
        }
    }

    private void HandleHit(Collider hitCollider)
    {
        var newInteractable = hitCollider.GetComponent<IInteractable>();
        if (newInteractable == null) return;

        if (newInteractable != _currentInteractable)
        {
            ClearSelection();
            _currentInteractable = newInteractable;
            _currentInteractable.OnSelected();
        }
    }

    private void ClearSelection()
    {
        if (_currentInteractable == null) return;

        // Check if object was destroyed before deselecting
        if (_currentInteractable is MonoBehaviour monoBehaviour && monoBehaviour == null)
        {
            _currentInteractable = null;
            return;
        }

        _currentInteractable.OnDeselected();
        _currentInteractable = null;
    }
}