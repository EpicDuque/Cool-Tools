// using System.Linq;
// using UnityEditor;
// using UnityEngine;
//
// namespace CoolTools.Actors.Editor
// {
//     [CustomEditor(typeof(AbilityBase))]
//     public class AbilityBaseEditor : UnityEditor.Editor
//     {
//         private AbilityBase _abilityBase;
//         private EffectBaseSearchWindow _searchWindow;
//
//         private void OnEnable()
//         {
//             _abilityBase = (AbilityBase) target;
//             
//             if(_searchWindow != null)
//                 _searchWindow.Close();
//         }
//
//         private void OnDisable()
//         {
//             if(_searchWindow != null)
//                 _searchWindow.Close();
//         }
//
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
//             
//             EditorGUILayout.Space(10f);
//             
//             // Draw a blue button with "Add Animation Effect" text
//
//             var originalColor = GUI.backgroundColor;
//             GUI.backgroundColor = new Color(0.47f, 0.75f, 1f, 0.77f);
//
//             if (GUILayout.Button("Embed Ability Effect", GUILayout.Height(25f)))
//             {
//                 EmbedAbilityEffect();
//             }
//             
//             GUI.backgroundColor = new Color(0.4f, 1f, 0.5f, 0.65f);
//             
//             EditorGUILayout.Space(20f);
//             
//             GUI.backgroundColor = new Color(1f, 0.23f, 0.23f);
//             
//             if(GUILayout.Button("Delete Selected Effects", GUILayout.Height(25f)))
//             {
//                 DeleteSelectedEffects();
//             }
//             
//             EditorGUILayout.Space(5f);
//             
//             GUI.backgroundColor = originalColor;
//         }
//         
//         private void DeleteSelectedEffects()
//         {
//             // Get all Objects in project selection
//             var selectedObjects = Selection.objects;
//             
//             var validObjects = selectedObjects
//                 .Where(o => o is EffectBase && AssetDatabase.IsSubAsset(o));
//             
//             // Delete all selected objects from project
//             foreach (var obj in validObjects)
//             {
//                 if(_abilityBase.AbilityEffects.Contains(obj as EffectBase))
//                     _abilityBase.DeleteAbilityEffect(obj as EffectBase);
//                 
//                 AssetDatabase.RemoveObjectFromAsset(obj);
//             }
//             
//             // Save the asset
//             AssetDatabase.SaveAssetIfDirty(_abilityBase);
//             AssetDatabase.Refresh();
//             
//             // Reimport the asset
//             AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(_abilityBase));
//         }
//
//         private void EmbedAbilityEffect()
//         {
//             if(_searchWindow != null)
//                 _searchWindow.Close();
//             
//             _searchWindow = EffectBaseSearchWindow.ShowWindow(instance =>
//             {
//                 if (string.IsNullOrEmpty(instance.name))
//                 {
//                     instance.name = instance.GetType().Name;
//                     AssetDatabase.AddObjectToAsset(instance, _abilityBase);
//                     _abilityBase.InsertAbilityEffect(instance);
//                 }
//                 else
//                 {
//                     var copy = Instantiate(instance);
//                     AssetDatabase.AddObjectToAsset(copy, _abilityBase);
//                     _abilityBase.InsertAbilityEffect(copy);
//                 }
//                 
//                 AssetDatabase.SaveAssetIfDirty(_abilityBase);
//                 
//             });
//         }
//     }
// }