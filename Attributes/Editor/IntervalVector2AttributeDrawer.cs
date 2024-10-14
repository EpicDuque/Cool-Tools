using UnityEditor;
using UnityEngine;

namespace CoolTools.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(IntervalVector2Attribute))]
    public class IntervalVector2AttributeDrawer : PropertyDrawer
    {
        private const float RangeValueWidth = 50;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var interval = attribute as IntervalVector2Attribute;

            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.LabelField(new Rect(position)
            {
                width = EditorGUIUtility.labelWidth
            }, label);

            var propertyBegin = position.x + EditorGUIUtility.labelWidth + 2;

            // Create basic rectangle
            var rect = new Rect(position)
            {
                x = propertyBegin,
                width = RangeValueWidth,
            };

            if (property.propertyType != SerializedPropertyType.Vector2)
            {
                rect.height = EditorGUIUtility.singleLineHeight * 2f;
                rect.width = position.width - propertyBegin - 5;
                EditorGUI.HelpBox(rect, "Use IntervalVector2 Attribute with a Vector2 property.", MessageType.Error);
                return;
            }
            
            var minVal = property.vector2Value.x;
            var maxVal = property.vector2Value.y;

            // Draw min value field
            minVal = EditorGUI.FloatField(rect, minVal);
            rect.x += rect.width + 5f;

            // Draw Slider
            rect.width = position.width - propertyBegin - (RangeValueWidth * 2) + 7;
            EditorGUI.MinMaxSlider(rect, ref minVal, ref maxVal, interval.min, interval.max);

            // Draw max value field
            rect.x += rect.width + 5f;
            rect.width = RangeValueWidth;
            maxVal = EditorGUI.FloatField(rect, maxVal);

            // Keep value presice in three decimal places
            maxVal = Mathf.Round(maxVal * 100f) / 100f;
            minVal = Mathf.Round(minVal * 100f) / 100f;

            maxVal = Mathf.Clamp(maxVal, minVal, interval.max);
            minVal = Mathf.Clamp(minVal, interval.min, maxVal);

            property.vector2Value = new Vector2(minVal, maxVal);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.propertyType == SerializedPropertyType.Vector2
                ? base.GetPropertyHeight(property, label)
                : EditorGUIUtility.singleLineHeight * 2f;
        }
    }
}