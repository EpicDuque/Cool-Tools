// using System.Globalization;
// using UnityEditor;
// using UnityEngine;
//
// namespace CoolTools.Actors.Editor
// {
//     public class FormulaEvaluatorWindow : EditorWindow
//     {
//         private Formula formula;
//         private StatProvider provider;
//         private FloatFormulaEval floatFormulaEval = new();
//         private IntFormulaEval intFormulaEval = new();
//
//         [MenuItem("CoolTools/Formula Evaluator")]
//         private static void ShowWindow()
//         {
//             var window = GetWindow<FormulaEvaluatorWindow>();
//             window.titleContent = new GUIContent("Formula Evaluator");
//             window.Show();
//         }
//
//         private void OnGUI()
//         {
//             EditorGUILayout.LabelField("Formula to Evaluate");
//             formula = EditorGUILayout.ObjectField(formula, typeof(Formula), false) as Formula;
//
//             EditorGUILayout.Space(10f);
//
//             EditorGUILayout.LabelField("Stat Provider");
//             provider = EditorGUILayout.ObjectField(provider, typeof(StatProvider), true) as StatProvider;
//
//             EditorGUILayout.Space(10f);
//
//             if (formula == null || provider == null)
//             {
//                 EditorGUILayout.HelpBox("Please assign both fields above", MessageType.Error);
//                 return;
//             }
//
//             floatFormulaEval.Formula = formula;
//             intFormulaEval.Formula = formula;
//
//             EditorGUI.BeginDisabledGroup(true);
//
//             DrawExpression();
//
//             DrawParsedExpression();
//
//             EditorGUILayout.Space(10f);
//
//             floatFormulaEval.Evaluate(provider);
//             intFormulaEval.Evaluate(provider);
//             DrawResult();
//
//             EditorGUI.EndDisabledGroup();
//         }
//
//         private void DrawResult()
//         {
//             EditorGUILayout.BeginHorizontal();
//
//             EditorGUILayout.PrefixLabel("Result");
//             EditorGUILayout.TextField(floatFormulaEval.Value.ToString(CultureInfo.InvariantCulture));
//
//             EditorGUILayout.EndHorizontal();
//
//             EditorGUILayout.BeginHorizontal();
//
//             EditorGUILayout.PrefixLabel("Rounded Result");
//             EditorGUILayout.TextField(intFormulaEval.Value.ToString());
//
//             EditorGUILayout.EndHorizontal();
//         }
//
//         private void DrawParsedExpression()
//         {
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.PrefixLabel("Parsed Expression");
//
//             // var parsed = floatFormulaEval.ParseExpression(provider);
//
//             // EditorGUILayout.TextField(parsed);
//
//             EditorGUILayout.EndHorizontal();
//         }
//
//         private void DrawExpression()
//         {
//             EditorGUILayout.BeginHorizontal();
//             EditorGUILayout.PrefixLabel("Expression");
//
//             // EditorGUILayout.TextField(formula.Expression);
//
//             EditorGUILayout.EndHorizontal();
//         }
//     }
// }
