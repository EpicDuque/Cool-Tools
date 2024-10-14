using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Data
{
    public class VariableEventListener<T, TY> : MonoBehaviour where T : VariableBase<TY>
    {
        [SerializeField] private T variable;
        
        [Space(10f)]
        public UnityEvent<TY> Response;

        protected void OnEnable()
        {
            variable.OnValueChanged += OnValueChanged;
        }
        
        protected void OnDisable()
        {
            variable.OnValueChanged -= OnValueChanged;
        }

        protected virtual void OnValueChanged(TY value)
        {
            Response?.Invoke(value);
        }
    }
}