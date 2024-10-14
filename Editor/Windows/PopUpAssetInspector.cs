using UnityEditor;
using UnityEngine;

namespace CoolTools.Editor
{
    public class PopUpAssetInspector : EditorWindow
    {
        private Object asset;
        private UnityEditor.Editor assetEditor;
        private Vector2 scrollPos;

        public static PopUpAssetInspector Create(Object asset)
        {
            if (asset == null) return null;

            var window = CreateWindow<PopUpAssetInspector>($"{asset.name} | {asset.GetType().Name}");
            window.asset = asset;
            window.assetEditor = UnityEditor.Editor.CreateEditor(asset);

            return window;
        }

        private void OnGUI()
        {
            GUI.enabled = false;
            asset = EditorGUILayout.ObjectField("Asset", asset, asset.GetType(), false);
            GUI.enabled = true;

            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            assetEditor.OnInspectorGUI();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        
        // Make this window always update its view when the asset changes
        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}