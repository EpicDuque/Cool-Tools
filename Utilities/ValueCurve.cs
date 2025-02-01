using CoolTools.Attributes;
using UnityEngine;

namespace CoolTools.Utilities
{
    [CreateAssetMenu(fileName = "New Value Curve", menuName = "Value Curve", order = 0)]
    public class ValueCurve : ScriptableObject
    {
        [SerializeField] private float scaleX = 1f;
        [SerializeField] private float scaleY = 1f;
        
        [Space(10f)]
        [SerializeField] private AnimationCurve curve;

        [Space(10f)] 
        [SerializeField] private float eval = 0.5f;
        [SerializeField] private bool roundToInt;
        
        [SerializeField, InspectorDisabled] private float result = 0f; 

        private void OnValidate()
        {
            result = Evaluate(eval);

            if (roundToInt)
                result = Mathf.RoundToInt(result);
        }

        public float Evaluate(float x) => curve.Evaluate(x * scaleX) * scaleY;
        
        public int EvaluateRounded(float x) => Mathf.RoundToInt(Evaluate(x));
    }
}