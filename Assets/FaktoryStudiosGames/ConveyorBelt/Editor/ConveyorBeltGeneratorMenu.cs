using UnityEngine;
using UnityEditor;

namespace FaktoryStudiosGames {
    public class ConveyorBeltGeneratorMenu
    {
        [MenuItem("GameObject/3D Object/Conveyor Belt", false, 10)]
        static void CreateTankTread()
        {
            GameObject go = new GameObject("Conveyor Belt");
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            var tread = go.AddComponent<ConveyorBelt>();

            // Optional: Assign default material (if you have one)
            var renderer = go.GetComponent<MeshRenderer>();
            var defaultMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/ConveyorBelt/Materials/ConveyorBeltMaterial.mat");
            if (defaultMat != null)
            {
                renderer.sharedMaterial = defaultMat;
                tread.material = defaultMat;
            }

            // Set as selected
            Selection.activeGameObject = go;

            // Set position nicely in the scene
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.MoveToView(go.transform);
            }
        }
    }
}