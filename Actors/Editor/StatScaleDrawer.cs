// using CoolTools.Actors;
// using UnityEditor;
// using UnityEngine;
//
// namespace CoolTools.Editor
// {
//     
//     [CustomPropertyDrawer(typeof(FormulaEval<>), true)]
//     public class StatScaleDrawer : PropertyDrawer
//     {
//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             var hasFormula = property.FindPropertyRelative("Formula").objectReferenceValue != null;
//             
//             return hasFormula ? base.GetPropertyHeight(property, label) * 2f + 10 : 
//                 base.GetPropertyHeight(property, label) + 2;
//         }
//
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             EditorGUI.BeginProperty(position, label, property);
//
//             position.height = EditorGUIUtility.singleLineHeight;
//
//             var propBaseValue = property.FindPropertyRelative("BaseValue");
//             var propFormula = property.FindPropertyRelative("Formula");
//             var propExpression = property.FindPropertyRelative("Expression");
//             var propValue = property.FindPropertyRelative("Value");
//
//             EditorGUI.LabelField(position, label);
//             
//             var rectBaseValueShort = new Rect(position)
//             {
//                 x = position.x + EditorGUIUtility.labelWidth,
//                 width = (position.width - EditorGUIUtility.labelWidth) * 0.35f,
//             };
//
//             var rectFormula = new Rect(position)
//             {
//                 x = rectBaseValueShort.xMax + 3,
//                 width = (position.width - EditorGUIUtility.labelWidth) * 0.65f - 3,
//             };
//             
//             var rectExpression = new Rect(position)
//             {
//                 y = position.y + EditorGUIUtility.singleLineHeight + 2,
//                 width = position.width * 0.5f - 5,
//             };
//
//             var rectValue = new Rect(position)
//             {
//                 y = position.y + EditorGUIUtility.singleLineHeight + 2,
//                 x = rectExpression.xMax + 10,
//                 width = position.width * 0.5f - 5,
//             };
//             
//             EditorGUI.PropertyField(rectBaseValueShort, propBaseValue, GUIContent.none);
//             EditorGUI.ObjectField(rectFormula, propFormula, GUIContent.none);
//
//             if (propFormula.objectReferenceValue != null)
//             {
//                 EditorGUI.PropertyField(rectExpression, propExpression, GUIContent.none);
//                 EditorGUI.PropertyField(rectValue, propValue, GUIContent.none);
//             }
//             
//             EditorGUI.EndProperty();
//         }
//     }
// }