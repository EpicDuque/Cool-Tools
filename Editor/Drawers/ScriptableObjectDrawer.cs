using CoolTools.Editor;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ScriptableObject), true)]
public class ScriptableObjectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var iconEdit = Resources.Load("CoolTools/Icons/Icon_Edit") as Texture;
        var iconNew = Resources.Load("CoolTools/Icons/Icon_New") as Texture;
        
        EditorGUI.BeginProperty(position, label, property);
        
        var isHovering = position.Contains(Event.current.mousePosition);
        
        // Get the rect of the label
        var controlRect = EditorGUI.PrefixLabel(position, label);
        EditorGUI.PropertyField(position, property);
        
        if (isHovering)
        {
            // position.x += width + 2;
            // Check if serializedProperty is null

            // Get the last used rect property
            var buttonPosition = new Rect(position)
            {
                x = controlRect.xMin - 45f,
                width = 45f,
            };
            if (property.objectReferenceValue != null)
            {
                if (GUI.Button(buttonPosition, iconEdit))
                {
                    PopUpAssetInspector.Create(property.objectReferenceValue);
                }
        
                return;
            }
        
            if (GUI.Button(buttonPosition, iconNew))
            {
                var typeName = property.type
                    .Replace("PPtr<$", "")
                    .Replace(">", "");
                
                var path = EditorUtility.SaveFilePanelInProject($"Create new {typeName}", 
                    $"New {typeName}", "asset", "Message");
        
                if (string.IsNullOrEmpty(path)) return;
                
                var instance = ScriptableObject.CreateInstance(typeName);
                
                AssetDatabase.CreateAsset(instance, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
        
                PopUpAssetInspector.Create(instance);
        
                property.objectReferenceValue = instance;
            }
        }
        
        EditorGUI.EndProperty();
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight + 3f;
    }
}
