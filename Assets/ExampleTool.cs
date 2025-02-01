// ExampleTool.cs
using UnityEngine;

public class ExampleTool : Tool
{
    [SerializeField] private float range = 5f;
    [SerializeField] private LayerMask interactableLayers;

    public override void Use()
    {
        base.Use();
       // PerformRaycast(userTransform);
        Debug.Log($"Example used");
    }

    private void PerformRaycast(Transform userTransform)
    {
        Ray ray = new Ray(userTransform.position, userTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, range, interactableLayers))
        {
            Debug.Log($"Hit {hit.collider.name} with ");
            // Add your interaction logic here
        }
    }
}