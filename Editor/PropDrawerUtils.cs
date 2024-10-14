using UnityEditor;
using UnityEngine;

namespace CoolTools.Editor
{
    public static class PropDrawerUtils
    {
        public static float LabelPropSpace = 5;
        public static float SizePerLabelChar = 10;
        public static float CurrentY = 0;

        private static Rect internalRect;

        public struct HorPropDrawerInfo
        {
            public SerializedProperty Property;
            public string PropName;
            public float WidthPct;
            public float LabelSize;
        }

        public static void Begin(Rect rect)
        {
            CurrentY = rect.y;
            internalRect = rect;
        }

        public static void NewLine() => CurrentY += EditorGUIUtility.singleLineHeight + 3;
        
        public static void DrawProperty(Rect rect, SerializedProperty property, string label = "", float labelSize = -1f, bool newLine = false)
        {
            var labelRect = new Rect(rect)
            {
                width = labelSize > 0 ? labelSize : label.Length * SizePerLabelChar,
                height = EditorGUIUtility.singleLineHeight,
                y = CurrentY,
            };
            var propRect = new Rect(rect)
            {
                width = rect.width - labelRect.width - LabelPropSpace,
                height = EditorGUIUtility.singleLineHeight,
                x = labelRect.xMax + LabelPropSpace,
                y = CurrentY,
            };
            
            EditorGUI.LabelField(labelRect, !string.IsNullOrEmpty(label) ? label : property.displayName);
            EditorGUI.PropertyField(propRect, property, GUIContent.none);

            if (newLine)
                NewLine();
        }

        public static void DrawProperty(SerializedProperty property, string label = "")
        {
            DrawProperty(internalRect, property, label, newLine:true);
        }

        public static void DrawPropertiesHorizontal(HorPropDrawerInfo[] drawerInfos, float space)
        {
            var rect = internalRect;
            
            var currentX = rect.x;
            var totalSpace = 0f;
            
            for (int i = 0; i < drawerInfos.Length; i++)
            {
                var info = drawerInfos[i];
                
                var propRect = new Rect(rect)
                {
                    width = rect.width * info.WidthPct - totalSpace,
                    x = currentX,
                };
                
                DrawProperty(propRect, info.Property, info.PropName, info.LabelSize);
                
                currentX = propRect.xMax + space;
                totalSpace += space;
            }
        }
    }
}