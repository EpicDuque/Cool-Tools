using System;
using UnityEngine;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New Delay Effect", menuName = "Actor/Effects/Delay", order = 0)]
    public class EffectDelay : EffectBase, IEffectWaiter
    {
        [SerializeField] private float _duration;

        private void OnEnable()
        {
            Wait = new WaitForSeconds(_duration);
        }

        public float Duration => _duration;
        
        public WaitForSeconds Wait { get; private set; }
    }
}