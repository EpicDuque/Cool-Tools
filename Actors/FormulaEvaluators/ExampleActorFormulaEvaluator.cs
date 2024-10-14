using System.Collections.Generic;
using UnityEngine;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New AdvancedActorFormulaEval", menuName = "Actor/Evaluators/Advanced Actor", order = 0)]
    public class ExampleActorFormulaEvaluator : ActorFormulaEvaluator
    {
        public override List<Formula.ParameterInput> GetInputParameters(Formula formula, Actor actor)
        {
            var inputParameters = base.GetInputParameters(formula, actor);
            
            var heightParameter = new Formula.ParameterInput
            {
                Name = "actor.y",
                Value = actor.transform.position.y
            };
            
            inputParameters.Add(heightParameter);

            return inputParameters;
        }
    }
}