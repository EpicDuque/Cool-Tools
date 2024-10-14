// using System;
// using System.Globalization;
// using UnityEngine;
//
// namespace CoolTools.Actors
// {
//     [Serializable]
//     public class FloatFormulaEval : FormulaEval<float>
//     {
//         public float Multiplier = 1f;
//         
//         protected override float CastResult(double result)
//         {
//             if (float.TryParse(result.ToString(CultureInfo.InvariantCulture), out var value))
//             {
//                 return (value * Multiplier) + BaseValue;
//             };
//             
//             Debug.LogError($"[{nameof(FloatFormulaEval)}] Failed to parse value: {result}");
//             return -1f;
//         }
//
//         protected override string ValueTypeToString(float value)
//         {
//             return value.ToString(CultureInfo.InvariantCulture);
//         }
//     }
// }