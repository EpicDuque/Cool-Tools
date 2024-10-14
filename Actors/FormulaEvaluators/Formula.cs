using System;
using System.Collections.Generic;
using System.Linq;
using B83.ExpressionParser;
using CoolTools.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New Formula", menuName = "Formula", order = 0)]
    public class Formula : ScriptableObject
    {
        [Serializable]
        public struct ParameterInput
        {
            [InspectorDisabled] public string Name;
            public double Value;
        }
        
        [SerializeField] private string rawExpression;
        
        [Space(10f)]
        [SerializeField, InspectorDisabled] private string parsedExpression;
        [SerializeField] private List<ParameterInput> inputParameters;
        
        [Space(10f)]
        [SerializeField, InspectorDisabled] private double result;
        
        private Expression cachedExpression;
        private ExpressionDelegate cachedDelegate;
        private ExpressionParser parser;

        public ParameterInput[] InputParameters => inputParameters.ToArray();

        public string RawExpression
        {
            get => rawExpression;
            private set => rawExpression = value;
        }

        public string ParsedExpression
        {
            get => parsedExpression;
            protected set => parsedExpression = value;
        }
        
        public double Result => result;

        private void OnValidate()
        {
            parser ??= new ExpressionParser();
        }

        private void OnEnable()
        {
            parser ??= new ExpressionParser();
            Parse();
        }

        public void Parse()
        {
            if (string.IsNullOrEmpty(rawExpression)) return;
            
            parser ??= new ExpressionParser();
            
            cachedExpression = parser.EvaluateExpression(rawExpression);
            
            ParsedExpression = cachedExpression.ToString();

            inputParameters.Clear();
            
            var parameters = cachedExpression.Parameters
                .Select(param => new ParameterInput { Name = param.Key, Value = param.Value.Value })
                .ToList();

            SetParameters(parameters);
        }

        private void SetParameters(IEnumerable<ParameterInput> parameters)
        {
            inputParameters.Clear();
            
            inputParameters.AddRange(parameters);
        }

        /// <summary>
        /// Evaluate parsed expression with existing parameters
        /// </summary>
        /// <returns></returns>
        public double Evaluate()
        {
            if(cachedExpression == null)
                Parse();

            if (cachedExpression == null) return 0f;
            
            var paramOrder = new string[inputParameters.Count];

            for (var i = 0; i < inputParameters.Count; i++)
            {
                var ip = inputParameters[i];
                paramOrder[i] = ip.Name;
            }
            
            cachedDelegate = cachedExpression.ToDelegate(paramOrder);

            var paramValues = new double[inputParameters.Count];
            for (int i = 0; i < inputParameters.Count; i++)
            {
                paramValues[i] = inputParameters[i].Value;
            }

            result = cachedDelegate.Invoke(paramValues);
            return result;
        }

        public double Evaluate(IEnumerable<ParameterInput> parameters)
        {
            SetParameters(parameters);

            return Evaluate();
        }
    }
}