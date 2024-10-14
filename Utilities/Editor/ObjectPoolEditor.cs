using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CoolTools.Utilities;

namespace CoolTools.Utilities.Editor
{
    [CustomEditor(typeof(ObjectPool))]
    public class ObjectPoolEditor : UnityEditor.Editor
    {
        private List<GameObject> objectsToAdd;
        private ObjectPool pool;

        private void OnEnable()
        {
            pool = target as ObjectPool;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.Space(10f);

            if (GUILayout.Button("Add Objects"))
            {
                pool.AddObjectListToPool();
                // Mark as dirty
                EditorUtility.SetDirty(pool);
                AssetDatabase.SaveAssetIfDirty(pool);
                AssetDatabase.SaveAssets();
            }
            
            EditorGUILayout.Space(10f);

            if (pool.Config == null) return;
            
            var editor = CreateEditor(pool.Config);
            editor.DrawDefaultInspector();
        }
    }
}
