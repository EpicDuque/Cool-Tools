using UnityEngine;

namespace CoolTools.Data
{
    [CreateAssetMenu(fileName = "New Void Variable", menuName = "Variable/Void")]
    public class VoidVariable : VariableBase<Void>
    {
        private Void emptyVoid;

        public void Raise()
        {
            Value = emptyVoid;
        }
    }
}
