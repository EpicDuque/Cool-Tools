// using System;
// using System.Globalization;
// using UnityEngine;
//
// namespace CoolTools.Actors
// {
//     [Serializable]
//     public class IntFormulaEval : FormulaEval<int>
//     {
//         public float Multiplier = 1f;
//         
//         protected override int CastResult(double result)
//         {
//             if (float.TryParse(result.ToString(CultureInfo.InvariantCulture), out var value))
//             {
//                 return Mathf.RoundToInt(value * Multiplier) + BaseValue;
//             };
//             
//             Debug.LogError($"[{nameof(IntFormulaEval)}] Failed to parse value: {result}");
//             
//             return -1;
//         }
//
//         protected override string ValueTypeToString(int value)
//         {
//             return value.ToString(CultureInfo.InvariantCulture);
//         }
//     }
// }