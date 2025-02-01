using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SBS.ME
{
	[CreateAssetMenu(fileName = "Guide", menuName = "SBS/ME/GUIDE")]
	public class GuideSO : ScriptableObject
	{
        public string nameAndVer = "MESH EXPLODER";
        public string smallName = "Mesh Exploder";
        public string docs = "MeshExploder_documentation";                                    
        public string onlineDocs = "https://www.smallbigsquare.com/documentation/meshexploder_documentation/index.html";
        public string tuts = "https://youtu.be/HDMbJEHcLYU";
        public string forum = "";
        public string mail = "smallbigsquare@gmail.com";
        public string rate = "https://assetstore.unity.com/packages/slug/307984";
        public string pub = "https://assetstore.unity.com/publishers/90279";
    }
}

namespace SBS.ME
{
    using System.Xml.Linq;
    using Unity.VisualScripting;
#if UNITY_EDITOR
    using UnityEditor;
    using TARGET = GuideSO;

	[CustomEditor(typeof(TARGET))]
    public class GuideSOEditor : Editor
    {
        private static GUIStyle titleStyle;
        private static GUIStyle subTitleStyle;
        private static GUIStyle regionStyle;
        private static GUIStyle bodyStyle;

        public static void setStyles()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(EditorStyles.label);
                titleStyle.fontSize = 20;
                titleStyle.fontStyle = FontStyle.Bold;
                titleStyle.alignment = TextAnchor.MiddleCenter;

                subTitleStyle = new GUIStyle(EditorStyles.label);
                subTitleStyle.fontSize = 15;
                subTitleStyle.fontStyle = FontStyle.Bold;
                subTitleStyle.alignment = TextAnchor.MiddleCenter;

                regionStyle = new GUIStyle(EditorStyles.label);
                regionStyle.fontSize = 16;
                regionStyle.fontStyle = FontStyle.Bold;

                bodyStyle = new GUIStyle(EditorStyles.label);
                bodyStyle.wordWrap = true;
                bodyStyle.fontSize = 13;
            }
        }

        public override void OnInspectorGUI()
        {
            setStyles();
            TARGET t = (TARGET)target;

            GUILayout.Label(t.nameAndVer, titleStyle);
            GUILayout.Label("GUIDE", subTitleStyle);
            GUILayout.Space(20);

            GUILayout.Label("Thank you for using " + t.smallName + "!", regionStyle);
            GUILayout.Label("Below you can find documentation. I also recommend you to run the example scene.", bodyStyle);

            if (GUILayout.Button(new GUIContent("Documentation", "PDF")) == true)
            {
                string[] find = AssetDatabase.FindAssets(t.docs);
                if (find.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(find[0]);

                    Application.OpenURL(Application.dataPath + path.Remove(0, "Assets".Length));
                }
            }

            if (GUILayout.Button(new GUIContent("Online Documentation", "Website")) == true)
            {
                Application.OpenURL(t.onlineDocs);
            }

            GUILayout.Space(20);

            GUILayout.Label("Need more help?", regionStyle);
            GUILayout.Label("You can watch tutorials, ask questions in forum or email me.", bodyStyle);

            if (GUILayout.Button(new GUIContent("Tutorials", "YouTube")) == true)
            {
                Application.OpenURL(t.tuts);
            }
                        
            if (GUILayout.Button(new GUIContent("E-Mail Me", t.mail)) == true)
            {
                Application.OpenURL("mailto:" + t.mail);
            }

            GUILayout.Space(20);

            GUILayout.Label("Rate the asset. THANKS!", regionStyle);
            GUILayout.Label("If you like the asset please rate it. It will help me to create more high quality assets.", bodyStyle);

            if (GUILayout.Button(new GUIContent("Rate This Asset", "Unity Asset Store")) == true)
            {
                Application.OpenURL(t.rate);
            }

            GUILayout.Space(20);

            GUILayout.Label("Want more?", regionStyle);
            GUILayout.Label("Check out my other assets. You may like them as well.", bodyStyle);

            if (GUILayout.Button(new GUIContent("MORE", "Unity Asset Store")) == true)
            {
                Application.OpenURL(t.pub);
            }

            GUILayout.Space(50);

            GUILayout.Label("NOTE", subTitleStyle);
            GUILayout.Label("Add example scenes to File/BuildSettings if you want 'Next Scene' button to work", bodyStyle);
        }
    }
#endif
}