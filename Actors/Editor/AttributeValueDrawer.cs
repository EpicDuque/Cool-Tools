using CoolTools.Actors;
using UnityEditor;
using UnityEngine;

namespace CoolTools.Editor
{
    [CustomPropertyDrawer(typeof(StatSheet.AttributeValue))]
    public class AttributeValueDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var statProp = property.FindPropertyRelative("Attribute");
            var valueProp = property.FindPropertyRelative("Value");
            
            var statRect = new Rect(position)
            {
                width = position.width * 0.75f,
                height = EditorGUIUtility.singleLineHeight,
                y = position.y,
            };
            
            EditorGUI.PropertyField(statRect, statProp, GUIContent.none);

            var space = 5f;
            var valueRect = new Rect(position)
            {
                width = position.width * 0.25f - space,
                height = EditorGUIUtility.singleLineHeight,
                x = statRect.xMax + space,
                y = position.y,
            };
            EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none);
            
            EditorGUI.EndProperty();
        }
    }
}