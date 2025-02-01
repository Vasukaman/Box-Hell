using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SBS.ME
{
#if UNITY_EDITOR
    /// <summary>
    /// Custom inspector for #SCRIPTNAME#
    /// </summary>
    [CustomEditor(typeof(MeshExploder))]
    public class MeshExploderEditor : Editor
    {
        //SerializedProperty exampleVar;        

        private void OnEnable()
        {
            //exampleVar = serializedObject.FindProperty("exampleVar"); //inspector val        
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update(); //update

            MeshExploder REF = (MeshExploder)target; //class reference           

            List<string> excluded = new List<string>();

            //drawing with exclusion2
            if (REF.createGameObjects == true)
            {                
                excluded.Add("gravity");
                excluded.Add("friction");
            }
            else
            {
                excluded.Add("onPartsCreated");
                excluded.Add("probabilityOfCreatingAnObject");                
                excluded.Add("colliderAttached");
            }

            if(REF.explosionOrigin!=ExplosionOrigin.offset)
            {
                excluded.Add("ExplosionOffset");
            }

            if (REF.useNormalsAsExplosionDirection == true)
            {
                excluded.Add("explosionOrigin");
                excluded.Add("ExplosionOffset");
                excluded.Add("normalizeDirection");
            }

            if(REF.doubleSided==false)
            {
                excluded.Add("flipSideDistance");
            }            

            DrawDefaultInspectorExcept(serializedObject, excluded);           

            serializedObject.ApplyModifiedProperties(); //update
        }

        /// <summary>
        /// Custom method to draw default inspector while excluding specific properties
        /// </summary>
        /// <param name="excludedProperties">Paroperties to exclude from drawing</param>
        public static void DrawDefaultInspectorExcept(SerializedObject serializedObject, List<string> excludedProperties)
        {
            SerializedProperty iterator = serializedObject.GetIterator();

            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (!excludedProperties.Contains(iterator.name) && iterator.name != "m_Script")
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
            }
        }
    }
#endif
}