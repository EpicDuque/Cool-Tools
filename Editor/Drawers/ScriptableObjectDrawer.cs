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
        
        EditorGUI.PropertyField(new Rect(position)
        {
            width = position.width * 0.9f,
        }, property, label);
        
        position.x += position.width * 0.9f + 2;
        
        // Check if serializedProperty is null
    
        if (property.objectReferenceValue != null)
        {
            if (GUI.Button(new Rect(position) { width = position.width * 0.1f, }, iconEdit))
            {
                PopUpAssetInspector.Create(property.objectReferenceValue);
            }
    
            return;
        }
        
        // position.x += position.width * 0.05f;
        
        if (GUI.Button(new Rect(position) { width = position.width * 0.1f, }, iconNew))
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
        
        EditorGUI.EndProperty();
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight + 3f;
    }
}
