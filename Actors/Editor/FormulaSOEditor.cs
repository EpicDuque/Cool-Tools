using UnityEditor;
using UnityEngine;

namespace CoolTools.Actors.Editor
{
    [CustomEditor(typeof(Formula))]
    public class FormulaSOEditor : UnityEditor.Editor
    {
        private Formula formula;
        private SerializedProperty rawExpressionProperty;
        private Vector2 scroll;

        private void OnEnable()
        {
            formula = (Formula) target;
            rawExpressionProperty = serializedObject.FindProperty("rawExpression");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();//
            
            // Get the Iterator then draw all the properties of the iterator
            var iterator = serializedObject.GetIterator();
            iterator.NextVisible(true);
            
            do
            {
                if (iterator.propertyPath == "m_Script") continue;
            
                if (iterator.propertyPath == "rawExpression")
                {
                    // Draw this text property with a blue color and bold text with the textField background color
                    var style = new GUIStyle(GUI.skin.textField)
                    {
                        richText = false,
                        normal = {textColor = new Color(0.48f, 1f, 0.71f)},
                        focused = {textColor = new Color(0.48f, 1f, 0.71f)},
                        hover = {textColor = new Color(0.48f, 1f, 0.71f)},
                        fontStyle = FontStyle.Normal,
                        fontSize = 13,
                        fixedHeight = 100,
                        font = Resources.Load<Font>("LUCON"),
                    };
                    // Draw the property displayName in front of this property
                    EditorGUILayout.LabelField(iterator.displayName);
                    
                    rawExpressionProperty.stringValue = EditorGUILayout.TextArea(rawExpressionProperty.stringValue, style);
                    
                    continue;
                }
            
                EditorGUILayout.PropertyField(iterator, true);
            } while (iterator.NextVisible(false));
            
            EditorGUILayout.Space(10f);
            
            EditorGUILayout.BeginHorizontal();

            var originalColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.42f, 0.6f, 0.73f);
            if (GUILayout.Button("Parse", GUILayout.Height(30f)))
            {
                formula.Parse();
            }
            
            GUI.backgroundColor = new Color(0.49f, 0.75f, 0.53f);
            
            if (GUILayout.Button("Evaluate", GUILayout.Height(30f)))
            {
                formula.Evaluate();
            }
            
            GUI.backgroundColor = originalColor;
            
            EditorGUILayout.EndHorizontal();
            
            serializedObject.ApplyModifiedProperties();
            
        }
    }
}