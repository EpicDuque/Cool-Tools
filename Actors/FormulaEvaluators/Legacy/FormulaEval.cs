// using System;
// using System.Linq;
// using CoolTools.Attributes;
//
// namespace CoolTools.Actors
// {
//     [Serializable]
//     public abstract class FormulaEval<T> where T : struct
//     {
//         public T BaseValue;
//         public Formula Formula;
//         
//         [InspectorDisabled] public string Expression;
//         [InspectorDisabled] public T Value;
//
//         public virtual T Evaluate(Actor actor)
//         {
//             if (actor != null) return Evaluate(actor.StatProvider);
//             
//             Value = BaseValue;
//             Expression = ValueTypeToString(BaseValue);
//             
//             return BaseValue;
//         }
//         
//         public virtual T Evaluate(StatProvider provider)
//         {
//             if (provider == null || Formula == null)
//             {
//                 Value = BaseValue;
//                 Expression = ValueTypeToString(BaseValue);
//                 return BaseValue;
//             }
//             
//             // Formula should already be parsed at this point.
//
//             // Look for Attribute Values that match the input parameters of the formula
//             var attributeValues = provider.CurrentStats
//                 .Where(av => Formula.InputParameters.Any(ip => ip.Name == av.Attribute.VariableName))
//                 .ToArray();
//
//             var inputParameters = attributeValues.Select(av => 
//                 new Formula.ParameterInput
//                 {
//                     Name = av.Attribute.VariableName,
//                     Value = provider.CalculateStatValue(av.Attribute)
//                 }).ToList();
//             
//             Expression = Formula.ParsedExpression;
//
//             Value = CastResult(Formula.Evaluate(inputParameters));
//
//             return Value;
//         }
//
//         protected abstract T CastResult(double result);
//         
//         protected abstract string ValueTypeToString(T value);
//     }
// }