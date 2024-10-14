using UnityEditor;
using UnityEngine;

namespace CoolTools.Actors.Editor
{
    [CustomEditor(typeof(ActorAbilityCaster))]
    public class ActorAbilityCasterEditor : UnityEditor.Editor
    {
        private ActorAbilityCaster caster;
        private AbilityBase testAbility;

        private void OnEnable()
        {
            caster = target as ActorAbilityCaster;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EditorGUILayout.Space(15f);

            testAbility = EditorGUILayout.ObjectField(testAbility, typeof(AbilityBase), false) as AbilityBase;
            
            if (GUILayout.Button("Cast Ability"))
            {
                caster.CastAbility(testAbility);
            }
        }
    }
}