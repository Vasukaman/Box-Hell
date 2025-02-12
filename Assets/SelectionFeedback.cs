using UnityEngine;

// Component to handle selection visual/audio feedback
public class SelectionFeedback : MonoBehaviour
{
    [SerializeField] private MonoBehaviour componentToToggle;
    [SerializeField] private GameObject uiIndicator;

    public void SetSelected(bool selected)
    {
        if (componentToToggle != null) componentToToggle.enabled = selected;
        if (uiIndicator != null) uiIndicator.SetActive(selected);
    }
}