using UnityEngine;

namespace CoolTools.Data
{
    [CreateAssetMenu(fileName = "New Float Variable", menuName = "Variable/Float")]
    public class FloatVariable : VariableBase<float>
    {
        public override float Value
        {
            get => base.Value;
            set
            {
                if (float.IsNaN(value)) return;
                base.Value = value;
            }
        }
    }
}