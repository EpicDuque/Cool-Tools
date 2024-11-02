using System;
using UnityEditor;
using UnityEngine;

namespace CoolTools.Actors
{
    [Serializable]
    public abstract class ValueConfig<T>
    {
        [SerializeField] private T _baseValue;
        [SerializeField] private Formula _formulaBaseValue;
        
        [SerializeField] private T _value;
        [SerializeField] private float _multiplier;
        [SerializeField] private T _offset;
        
        public T BaseValue
        {
            get => _baseValue;
            set => _baseValue = value;
        }
        
        public T Value
        {
            get => _value;
            set => _value = value;
        }
        
        public float Multiplier
        {
            get => _multiplier;
            set => _multiplier = value;
        }
        
        public T Offset
        {
            get => _offset;
            set => _offset = value;
        }
        
        public Formula FormulaBaseValue
        {
            get => _formulaBaseValue;
            set => _formulaBaseValue = value;
        }

        public abstract void UpdateValue();

        public virtual void UpdateValue(OwnableBehaviour ownable)
        {
            if (Application.isPlaying)
            {
                if(ownable.HasOwner) UpdateValue(ownable.Owner);
                else UpdateValue();
            }
            else
            {
                if(ownable.Owner != null) 
                    UpdateValue(ownable.Owner);
                else UpdateValue();
            }
        }

        public virtual void UpdateValue(Actor owner)
        {
            if (owner.Evaluator == null)
            {
                UpdateValue();
                return;
            }
            
            UpdateValue(owner.Evaluator, new EvaluateParams { Source = owner, Target = null});
        }

        public abstract void UpdateValue(FormulaEvaluator evaluator, EvaluateParams evaluateParams);
    }
    
    [Serializable]
    public class FloatValueConfig : ValueConfig<float>
    {
        public override void UpdateValue()
        {
            Value = BaseValue * Multiplier + Offset;
        }

        public override void UpdateValue(FormulaEvaluator evaluator, EvaluateParams evaluateParams)
        {
            if (FormulaBaseValue != null)
            {
                evaluateParams.Formula = FormulaBaseValue;
                evaluateParams.FallBackValue = BaseValue;
                
                BaseValue = evaluator.Evaluate(evaluateParams);
            }
            
            UpdateValue();
        }
    }//
    
    [Serializable]
    public class IntValueConfig : ValueConfig<int>
    {
        public override void UpdateValue()
        {
            Value = Mathf.RoundToInt(BaseValue * Multiplier + Offset);
        }
        
        public override void UpdateValue(FormulaEvaluator evaluator, EvaluateParams evaluateParams)
        {
            if (FormulaBaseValue != null)
            {
                evaluateParams.Formula = FormulaBaseValue;
                evaluateParams.FallBackValue = BaseValue;
                
                BaseValue = Mathf.RoundToInt(evaluator.Evaluate(evaluateParams));
            }
            
            UpdateValue();
        }
    }
}