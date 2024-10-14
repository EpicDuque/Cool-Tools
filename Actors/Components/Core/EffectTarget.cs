using UnityEngine;

namespace CoolTools.Actors
{
    public class EffectTarget : MonoBehaviour
    {
        [SerializeField] private EffectTargetTag targetTag;
        
        public EffectTargetTag TargetTag => targetTag;
    }
}