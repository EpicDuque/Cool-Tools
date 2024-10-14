using System;
using CoolTools.Attributes;
using UnityEngine;

namespace CoolTools.Data
{
    /// <summary>
    /// This ScriptableObject represents a value of any type, serialized into an asset.
    /// MonoBehaviour components can read and write values into these assets. Game systems
    /// can then subscribe to these variables to recieve change updates.
    ///
    /// More Info: https://web.archive.org/web/20220716032546/https://unity.com/how-to/architect-game-code-scriptable-objects
    /// </summary>
    public class VariableBase<T> : VariableBase
    {
        [SerializeField] protected T value;
        [SerializeField] protected T defaultValue;

        [Space(10f)] 
        [SerializeField, TextArea] private string description;
        
        public virtual T Value
        {
            get => value;
            set => this.value = value;
        }

        protected void OnEnable()
        {
            Reset();
        }
        
        public override void Reset()
        {
            Value = defaultValue;
        }

    }

    public class VariableBase : ScriptableObject
    {
        public virtual void Reset()
        {
        }
    }
}