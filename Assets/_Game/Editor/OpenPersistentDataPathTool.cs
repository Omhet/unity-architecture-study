using UnityEditor;
using UnityEngine;

namespace App.Editor
{
    public static class OpenPersistentDataPathTool
    {
        [MenuItem("Tools/Open Persistent Data Path")]
        public static void Open()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
    }
}
