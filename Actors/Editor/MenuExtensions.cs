using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoolTools.Actors
{
    public static class MenuExtensions
    {
        [MenuItem("GameObject/Actor/New Empty Actor", false, 0)]
        public static void AddNewActor()
        {
            var components = new[]
            {
                typeof(Actor),
                typeof(StatProvider)
            };
            
            var actor = new GameObject("Actor", components);
            var model = new GameObject("Model", typeof(Animator), typeof(ActorAnimationEventListener));
            
            var animator = model.GetComponent<Animator>();
            animator.applyRootMotion = false;
            
            model.transform.SetParent(actor.transform);
            model.transform.localPosition = Vector3.zero;
            
            Undo.RegisterCreatedObjectUndo(actor, "Create " + actor.name);
            Selection.activeObject = actor;
        }
        
        [MenuItem("GameObject/Actor/New Player Actor", false, 0)]
        public static void AddNewPlayerActor()
        {
            var components = new[]
            {
                typeof(Actor),
                typeof(StatProvider),
                typeof(DamageableResource),
                typeof(CharacterController)
            };
            
            var actorObj = new GameObject("Player Actor", components);
            var model = new GameObject("Model", typeof(Animator), typeof(ActorAnimationEventListener));

            var actor = actorObj.GetComponent<Actor>();
            var provider = actorObj.GetComponent<StatProvider>();
            var damagebale = actorObj.GetComponent<DamageableResource>();
            var cc = actorObj.GetComponent<CharacterController>();

            actor.StatProvider = provider;
            actor.DamageableResource = damagebale;

            var animator = model.GetComponent<Animator>();
            animator.applyRootMotion = false;
            
            model.transform.SetParent(actorObj.transform);
            model.transform.localPosition = Vector3.zero;
            
            Undo.RegisterCreatedObjectUndo(actorObj, "Create " + actor.name);
            Selection.activeObject = actorObj;
        }

        [MenuItem("Tools/CoolTools/Bake Evaluators", false, 0)]
        public static void BakeAllParametersIntoEvaluators()
        {
            var guids = AssetDatabase.FindAssets("t:FormulaEvaluator");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var evaluator = AssetDatabase.LoadAssetAtPath<FormulaEvaluator>(path);
                evaluator.ClearBakedParameterNames();
                evaluator.BakeParameters();
                
                EditorUtility.SetDirty(evaluator);
            }
            
            AssetDatabase.SaveAssets();
        }
    }
}