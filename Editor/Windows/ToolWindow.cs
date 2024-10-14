using UnityEditor;
using UnityEngine;

namespace CoolTools.Editor
{
    public abstract class ToolWindow : EditorWindow
    {
        protected void DrawSeparator(float spaceBefore = 25f, float spaceAfter = 15f)
        {
            var rect = new Rect(GUILayoutUtility.GetLastRect())
            {
                height = 2,
            };
            rect.y = rect.yMax + spaceBefore;

            EditorGUI.DrawRect(rect, new Color(0.4f, 0.4f, 0.4f));

            EditorGUILayout.Space(spaceAfter);
        }

        protected void DrawTransformField(ref Transform value, string prefix = "", float prefixSize = -1f)
        {
            EditorGUILayout.BeginHorizontal();

            var width = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = prefixSize < 0 ? prefix.Length * 10 : prefixSize;

            EditorGUILayout.PrefixLabel(prefix);
            value = (Transform)EditorGUILayout.ObjectField(value, typeof(Transform), true);

            EditorGUIUtility.labelWidth = width;

            EditorGUILayout.EndHorizontal();
        }

        protected void DrawField<T>(ref T value, string prefix = "", float prefixSize = -1f) where T : Object
        {
            EditorGUILayout.BeginHorizontal();

            var width = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = prefixSize < 0 ? prefix.Length * 10 : prefixSize;

            EditorGUILayout.PrefixLabel(prefix);
            value = (T)EditorGUILayout.ObjectField(value, typeof(T), true);

            EditorGUIUtility.labelWidth = width;

            EditorGUILayout.EndHorizontal();
        }

        protected void DrawTransformField(Rect rect, ref Transform value, string prefix = "")
        {

        }
    }
}
