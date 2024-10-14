using CoolTools.Attributes;
using UnityEditor;
using UnityEngine;

namespace CoolTools.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxAttributeDrawer : PropertyDrawer
    {
        private const float CharacterWidth = 7f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var helpBox = attribute as HelpBoxAttribute;

            EditorGUI.BeginProperty(position, label, property);

            var helpBoxHeight = GetHelpBoxHeight(helpBox.Text);

            EditorGUI.HelpBox(new Rect(position) {height = helpBoxHeight}, helpBox.Text, MessageType.Info);

            position.y += helpBoxHeight + 5f;

            var propertyHeight = base.GetPropertyHeight(property, label);
            var rect = new Rect(position)
            {
                height = propertyHeight
            };

            EditorGUI.PropertyField(rect, property, label);

            EditorGUI.EndProperty();
        }

        private float GetHelpBoxHeight(string text)
        {
            var lines = Mathf.CeilToInt(text.Length * CharacterWidth / Screen.width * 1.5f);

            return Mathf.Clamp(lines, 2, float.MaxValue) * EditorGUIUtility.singleLineHeight;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var helpBox = attribute as HelpBoxAttribute;
            return base.GetPropertyHeight(property, label) + GetHelpBoxHeight(helpBox.Text) + 5;
        }
    }
}