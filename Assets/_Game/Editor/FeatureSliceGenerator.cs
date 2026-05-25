#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

namespace App.Editor
{
    public class FeatureSliceGenerator : EditorWindow
    {
        private string _featureName = "NewFeature";

        [MenuItem("Tools/Architecture/Create Feature Slice")]
        public static void ShowWindow()
        {
            GetWindow<FeatureSliceGenerator>("Create Feature Slice");
        }

        private void OnGUI()
        {
            GUILayout.Label("Create a Clean Architecture Feature Slice", EditorStyles.boldLabel);
            _featureName = EditorGUILayout.TextField("Feature Name", _featureName);

            if (GUILayout.Button("Generate"))
            {
                GenerateFeatureSlice(_featureName);
                Close();
            }
        }

        private void GenerateFeatureSlice(string featureName)
        {
            if (string.IsNullOrWhiteSpace(featureName))
            {
                Debug.LogError("Feature name cannot be empty.");
                return;
            }

            string basePath = $"Assets/_Game/Features/{featureName}";

            // Map of subfolder to target asmdef reference
            var layerMap = new (string folder, string refName)[]
            {
                ("Core", "App.Core"),
                ("View", "App.View"),
                ("Flow", "App.Flow")
            };

            foreach (var layer in layerMap)
            {
                string folderPath = $"{basePath}/{layer.folder}";
                Directory.CreateDirectory(folderPath);

                string asmrefPath = $"{folderPath}/{featureName}.{layer.folder}.asmref";
                string jsonContent = $"{{\n    \"reference\": \"{layer.refName}\"\n}}";

                File.WriteAllText(asmrefPath, jsonContent);
            }

            AssetDatabase.Refresh();
            Debug.Log($"Successfully generated feature slice: {featureName} at {basePath}");
        }
    }
}
#endif