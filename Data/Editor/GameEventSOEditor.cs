using CoolTools.Data;
using UnityEditor;
using UnityEngine;

namespace CoolTools.Editor
{
    [CustomEditor(typeof(GameEventSO<>), true)]
    public class GameEventSOEditor<T, TE> : UnityEditor.Editor where T : GameEventSO<TE>
    {
        protected T gameEvent;

        private void OnEnable()
        {
            gameEvent = target as T;
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Raise"))
            {
                TestRaise();
            }
            
            EditorGUILayout.Space();
            
            base.OnInspectorGUI();
        }
        
        public virtual void TestRaise() => gameEvent.Raise(gameEvent.TestData);
    }
}