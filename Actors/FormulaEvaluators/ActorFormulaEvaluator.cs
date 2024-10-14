using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New Actor Formula Evaluator", menuName = "Actor/Evaluators/Actor", order = 0)]
    public class ActorFormulaEvaluator : FormulaEvaluator
    {
        public virtual float Evaluate(Formula formula, Actor actor, float fallBackValue = 0f, float baseValue = 0f)
        {
            if (actor == null || formula == null)
            {
                return baseValue;
            }
            
            var inputParameters = GetInputParameters(formula, actor);
        
            return base.Evaluate(formula, inputParameters, fallBackValue, baseValue);
        }
        
        public virtual List<Formula.ParameterInput> GetInputParameters(Formula formula, Actor actor)
        {
            if (actor == null) return null;
            
            var attributeValues = actor.StatProvider.CurrentStats
                .Where(av => formula.InputParameters.Any(ip => ip.Name == av.Attribute.VariableName))
                .ToArray();
        
            return attributeValues.Select(av => 
                new Formula.ParameterInput
                {
                    Name = av.Attribute.VariableName,
                    Value = actor.StatProvider.CalculateStatValue(av.Attribute)
                }).ToList();
        }
    }
}