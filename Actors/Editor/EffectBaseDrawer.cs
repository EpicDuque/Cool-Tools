using UnityEditor;
using UnityEngine;

namespace CoolTools.Actors.Editor
{
    [CustomPropertyDrawer(typeof(EffectBase), true)]
    public class EffectBaseDrawer : PropertyDrawer
    {
        private EffectBaseSearchWindow _searchWindow;
        // private Dictionary<SerializedProperty, bool> _foldoutStates = new();
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var originalColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.49f, 1f, 0.58f, 0.77f);
            var effectPropertyRect = new Rect(position)
            {
                width = position.width,
                height = EditorGUIUtility.singleLineHeight + 10f,
            };
            EditorGUI.PropertyField(effectPropertyRect, property, label);
            
            
            GUI.backgroundColor = originalColor;

            var asset = property.objectReferenceValue;
            if (asset != null)
            {
                var assetEditor = UnityEditor.Editor.CreateEditor(asset);
                var iterator = assetEditor.serializedObject.GetIterator();
                
                position.y += EditorGUIUtility.singleLineHeight + 10f;
                position.y += EditorGUIUtility.singleLineHeight - 5f;
                
                GUI.enabled = false;
                iterator.NextVisible(true);
                EditorGUI.PropertyField(position, iterator, true);
                GUI.enabled = true;
                
                position.y += EditorGUI.GetPropertyHeight(iterator, true) + 5f;
                
                // Watch and apply editor changes
                
                while (iterator.NextVisible(false))
                {
                    var propertyHeight = EditorGUI.GetPropertyHeight(iterator, true);
                    var propertyRect = new Rect(position)
                    {
                        height = propertyHeight,
                    };
                    
                    EditorGUI.PropertyField(propertyRect, iterator, true);
                    
                    // Increase Y by the height of the last property
                    position.y += propertyHeight + 2f;
                }

                assetEditor.serializedObject.ApplyModifiedProperties();
                EditorGUI.EndFoldoutHeaderGroup();
            }
            
            EditorGUI.EndProperty();
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = base.GetPropertyHeight(property, label) + 10f;
            var asset = property.objectReferenceValue;

            if (asset != null)
            {
                height += EditorGUIUtility.singleLineHeight + 5f;
                var assetEditor = UnityEditor.Editor.CreateEditor(asset);
                
                // Get the total height of the editor
                var iterator = assetEditor.serializedObject.GetIterator();
                
                iterator.NextVisible(true); 
                height += EditorGUI.GetPropertyHeight(iterator, true) + 2f;
                while (iterator.NextVisible(false))
                {
                    height += EditorGUI.GetPropertyHeight(iterator, true) + 2f;
                }
            }

            return height;
        }
    }
}