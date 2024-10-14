using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Data
{
    public class FloatVariableListener : VariableEventListener<FloatVariable, float>
    {
        [SerializeField] private bool allValues = true;
        [SerializeField] private float valueToListen;
        
        public UnityEvent<float> OnValueGreaterThan;
        public UnityEvent<float> OnValueLessThan;

        protected override void OnValueChanged(float value)
        {
            base.OnValueChanged(value);
            
            if (allValues) return;
            
            if(value > valueToListen)
                OnValueGreaterThan?.Invoke(value);
            else if(value < valueToListen)
                OnValueLessThan?.Invoke(value);
        }
    }
}