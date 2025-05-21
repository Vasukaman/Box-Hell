using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class ProgressIndicator : MonoBehaviour
{
    [Header("Element Prefabs")]
    [SerializeField] private GameObject filledElementPrefab;
    [SerializeField] private GameObject emptyElementPrefab;

    [Header("Layout Settings")]
    [SerializeField] private HorizontalLayoutGroup layoutGroup;
    [Tooltip("If true, will always show max elements. If false, only shows current count")]
    [SerializeField] private bool showEmptyElements = true;

    private void Awake()
    {
        if (layoutGroup == null)
            layoutGroup = GetComponent<HorizontalLayoutGroup>();
    }

    /// <summary>
    /// Update progress display with current/max values
    /// </summary>
    /// <param name="current">Number of filled elements</param>
    /// <param name="max">Total number of elements to display</param>
    public void UpdateProgress(int current, int max)
    {
        ClearElements();

        current = Mathf.Clamp(current, 0, max);
        max = Mathf.Max(max, 1);

        int elementsToShow = showEmptyElements ? max : current;

        for (int i = 0; i < elementsToShow; i++)
        {
            bool shouldFill = i < current;
            GameObject elementPrefab = shouldFill ? filledElementPrefab : emptyElementPrefab;

            if (elementPrefab != null)
            {
                Instantiate(elementPrefab, layoutGroup.transform);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
    }

    private void ClearElements()
    {
        foreach (Transform child in layoutGroup.transform)
        {
            Destroy(child.gameObject);
        }
    }
}