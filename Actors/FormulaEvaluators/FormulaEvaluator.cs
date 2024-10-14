using System.Collections.Generic;

using UnityEngine;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New Formula Evaluator", menuName = "Actors/Evaluators/Generic", order = 0)]
    public class FormulaEvaluator : ScriptableObject
    {
        public virtual float Evaluate(Formula formula, IEnumerable<Formula.ParameterInput> parameters, float fallBackValue = 0f, float baseValue = 0f)
        {
            return formula != null ? (float) formula.Evaluate(parameters) + baseValue : fallBackValue;
        }
    }
    
}