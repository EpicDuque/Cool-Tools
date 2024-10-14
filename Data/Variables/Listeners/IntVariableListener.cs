using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Data
{
    public class IntVariableListener : VariableEventListener<IntVariable, int>
    {
        [SerializeField] private bool allValues = true;
        [SerializeField] private int valueToListen;

        public UnityEvent<int> OnValueEqual;
        public UnityEvent<int> OnValueGreaterThan;
        public UnityEvent<int> OnValueLessThan;

        protected override void OnValueChanged(int value)
        {
            base.OnValueChanged(value);
            
            if (allValues) return;
            
            if(value == valueToListen)
                OnValueEqual?.Invoke(value);
            else if(value > valueToListen)
                OnValueGreaterThan?.Invoke(value);
            else
                OnValueLessThan?.Invoke(value);
        }
    }
}