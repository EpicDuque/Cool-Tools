using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CoolTools.Editor
{
    public class AssetEmbedWindow : EditorWindow
    {
        private Object _targetAsset;
        private bool _withCopy;

        [MenuItem("Tools/CoolTools/Asset Embed Tool")]
        private static void ShowWindow()
        {
            var window = GetWindow<AssetEmbedWindow>();
            window.titleContent = new GUIContent("Asset Embed Tool");
            window.Show();
        }

        private void OnGUI()
        {
            // Draw an object field for a target asset in the project (no hierarchy)
            _targetAsset = EditorGUILayout.ObjectField("Target Asset", _targetAsset, typeof(Object), false);

            // Make a boolean field to indicate if copy of the assets should be embedded instead
            _withCopy = EditorGUILayout.Toggle("Embed with Copy", _withCopy);

            if (Selection.objects.Length > 0)
            {
                var originalColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.47f, 0.75f, 1f, 0.77f);
                
                // Draw a button with "Embed Asset" text
                if (_targetAsset != null && !Selection.objects.Contains(_targetAsset) &&
                    GUILayout.Button("Embed Selected Assets", GUILayout.Height(25f)))
                {
                    EmbedAssets(_targetAsset, _withCopy);
                }
                
                // Make a list of selected assets that are subassets of targetAsset
                var subAssets = Selection.objects
                    .Where(AssetDatabase.IsSubAsset).ToArray();
                
                GUI.backgroundColor = new Color(1f, 0.23f, 0.23f);
                if (subAssets.Length > 0 && 
                    GUILayout.Button("Delete Selected Sub-Assets", GUILayout.Height(25f)))
                {
                    DeleteSelectedSubAssets(subAssets, _targetAsset);
                }
                
                GUI.backgroundColor = originalColor;
            }
        }

        private void DeleteSelectedSubAssets(Object[] selectedSubAssets, Object targetAsset)
        {
            foreach (var asset in selectedSubAssets)
            {
                AssetDatabase.RemoveObjectFromAsset(asset);
            }
            
            AssetDatabase.SaveAssetIfDirty(targetAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void EmbedAssets(Object targetAsset, bool withCopy = false)
        {
            // Get all Objects in project selection and add them as subassets of targetAsset then save
            var selectedAssets = Selection.objects;
            foreach (var asset in selectedAssets)
            {
                if(asset == _targetAsset) continue;
                
                var copy = Instantiate(asset);
                AssetDatabase.AddObjectToAsset(copy, targetAsset);

                if (!withCopy)
                {
                    var path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                }
            }
            
            AssetDatabase.SaveAssetIfDirty(targetAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}