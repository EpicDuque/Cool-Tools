using UnityEngine;

namespace CoolTools.Data
{
    [CreateAssetMenu(fileName = "New Int Variable", menuName = "Variable/Int")]
    public class IntVariable : VariableBase<int>
    {
        [SerializeField] private int minValue = int.MinValue;
        [SerializeField] private int maxValue = int.MaxValue;

        public override int Value
        {
            get => base.Value;
            set => base.Value = Mathf.Clamp(value, minValue, maxValue);
        }

        public void Increase(int amount) => Value += amount;

        public void Decrease(int amount) => Value -= amount;
        
        public void Multiply(float amount) => Value = Mathf.RoundToInt(Value * amount);
        
    }
}