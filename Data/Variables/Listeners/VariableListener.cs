using CoolTools.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Data
{
    public class VariableListener<T> : MonoBehaviour
    {
        [SerializeField] private VariableBase<T> variable;

        public UnityEvent<T> OnWireValueChanged;

        [field: SerializeField, InspectorDisabled] public T LastValue { get; set; }
        
        private void OnEnable()
        {
            variable.OnValueChanged += VariableValueChange;
        }

        private void OnDisable()
        {
            variable.OnValueChanged -= VariableValueChange;
        }

        protected virtual void VariableValueChange(T v)
        {
            LastValue = v;
            OnWireValueChanged?.Invoke(v);
        }
    }
}