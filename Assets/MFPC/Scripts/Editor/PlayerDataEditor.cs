using UnityEditor;
using UnityEngine;

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using MFPC.Utils;

namespace MFPC.EditorScripts
{
    [CustomEditor(typeof(PlayerData))]
    public class PlayerDataEditor : Editor
    {
        private PlayerData playerData;
        private Dictionary<string, List<PropertyInfo>> headerProperties;

        private void OnEnable()
        {
            playerData = (PlayerData)target;
            var properties = playerData.GetType().GetProperties();

            headerProperties = new Dictionary<string, List<PropertyInfo>>();
            List<PropertyInfo> currentHeaderFields = null;

            foreach (var property in properties)
            {
                var headerAttribute = property.GetCustomAttribute<HeaderData>();

                if (headerAttribute != null)
                {
                    if (!headerProperties.ContainsKey(headerAttribute.header))
                    {
                        headerProperties[headerAttribute.header] = new List<PropertyInfo>();
                    }

                    currentHeaderFields = headerProperties[headerAttribute.header];
                }

                if (currentHeaderFields != null)
                {
                    currentHeaderFields.Add(property);
                }
            }
        }

        public override void OnInspectorGUI()
        {
            DrawChoseStateArea();

            int sectionIndex = 0;
            foreach (var kvp in headerProperties)
            {
                DrawChosenStateArea(sectionIndex, kvp.Key, kvp.Value);

                sectionIndex++;
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(playerData);
            }
        }

        private void DrawChoseStateArea()
        {
            GUILayout.BeginVertical("box");
            if (!Utils.EnumUtils.IsMaximumSelection(playerData.AvailablePlayerStates))
                DrawLabel("Selecting the available player state", TextAnchor.MiddleCenter);

            foreach (PlayerStates state in Enum.GetValues(typeof(PlayerStates)))
            {
                if ((playerData.AvailablePlayerStates & state) == 0)
                {
                    if (GUILayout.Button(state.ToString()))
                        SetAvailableStates(playerData.AvailablePlayerStates | state);
                }
            }

            GUILayout.EndVertical();
        }

        private void DrawChosenStateArea(int sectionIndex, string header, List<PropertyInfo> properties)
        {
            bool isAvailable = false;
            PlayerStates currentState = PlayerStates.Move;

            foreach (PlayerStates state in Enum.GetValues(playerData.AvailablePlayerStates.GetType()).Cast<Enum>()
                         .Where(playerData.AvailablePlayerStates.HasFlag))
            {
                if (header == state.ToString())
                {
                    currentState = state;
                    isAvailable = true;
                    break;
                }
            }

            if (isAvailable)
            {
                GUILayout.BeginVertical("box");
                GUILayout.BeginVertical("box");
                DrawLabel($"{header} State", TextAnchor.MiddleCenter);
                GUILayout.EndVertical();
                foreach (var property in properties)
                {
                    if (property.Name == "name" || property.Name == "hideFlags") continue;

                    object propertyValue = property.GetValue(playerData);
                    propertyValue = Utils.FieldDrawer.DrawField(propertyValue, property.PropertyType, property.Name);
                    property.SetValue(playerData, propertyValue);
                }

                if (sectionIndex != 0 && GUILayout.Button("Remove"))
                    SetAvailableStates(playerData.AvailablePlayerStates & ~currentState);

                GUILayout.EndVertical();
            }
        }

        private void DrawLabel(string text, TextAnchor textAnchor)
        {
            GUIStyle centeredLabelStyle = new GUIStyle();

            centeredLabelStyle = new GUIStyle(GUI.skin.label);
            centeredLabelStyle.alignment = textAnchor;
            centeredLabelStyle.fontStyle = FontStyle.Bold;

            GUILayout.Label(text, centeredLabelStyle);
        }

        private void SetAvailableStates(PlayerStates newStates)
        {
            PropertyInfo propertyInfo = typeof(PlayerData).GetProperty("AvailablePlayerStates",
                BindingFlags.Public | BindingFlags.Instance);

            propertyInfo.SetValue(playerData, newStates);
        }
    }
}