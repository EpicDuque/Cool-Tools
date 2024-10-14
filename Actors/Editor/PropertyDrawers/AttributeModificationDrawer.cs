using CoolTools.Actors;
using UnityEditor;
using UnityEngine;

namespace CoolTools.Editor
{
    [CustomPropertyDrawer(typeof(StatProvider.AttributeModification))]
    public class AttributeModificationDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();

            var attributeProp = property.FindPropertyRelative("Attribute");
            var offsetProp = property.FindPropertyRelative("Offset");
            var multProp = property.FindPropertyRelative("Multiplier");
            
            var statRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight,
            };
            
            EditorGUI.PropertyField(statRect, attributeProp, GUIContent.none);

            if (attributeProp.objectReferenceValue == null)
            {
                EditorGUI.EndProperty();
                return;
            }
            
            position.y += 2;
            
            // Draw Offset Property
            var oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 40;
            var offsetRect = new Rect(position)
            {
                y = position.y + EditorGUIUtility.singleLineHeight + 1,
                height = EditorGUIUtility.singleLineHeight,
                width = position.width/2f - 5,
            };
            EditorGUI.PropertyField(offsetRect, offsetProp, new GUIContent("Offset"));
            
            // Draw Multiplier Property
            EditorGUIUtility.labelWidth = 60;
            var multRect = new Rect(position)
            {
                y = position.y + EditorGUIUtility.singleLineHeight + 1,
                height = EditorGUIUtility.singleLineHeight,
                width = position.width/2f - 5,
                x = offsetRect.xMax + 10,
            };
            EditorGUI.PropertyField(multRect, multProp, new GUIContent("Multiplier"));
            
            EditorGUIUtility.labelWidth = oldWidth;
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attributeProp = property.FindPropertyRelative("Attribute");

            return attributeProp.objectReferenceValue != null ? 
                (EditorGUIUtility.singleLineHeight * 2) + 5 : 
                EditorGUIUtility.singleLineHeight;
        }
    }
}