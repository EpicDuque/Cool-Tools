using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Actor/Ability")]
    public class AbilityBase : ScriptableObject
    {
        [Serializable]
        public struct EventEffect
        {
            [HideInInspector] public string EventName;
            public AnimationEventSO Event;
            
            public EffectBase[] Effects;
        }
        
        [SerializeField] protected string _abilityName;
        
        [Space(10f)]
        [SerializeField] protected float _cooldown;
        [Tooltip("Specify a duration for the ability. -1 means ability ends specifically with AnimationEvent")]
        [SerializeField] private float _abilityDuration;
        [SerializeField] private bool _canBeInterrupted;
        
        [Header("Animation Params")] 
        [SerializeField] private bool _setTrigger;
        [SerializeField] private string _triggerName;
        
        [Space(10f)] 
        [SerializeField] protected ResourceCost[] _resourceCosts;
        
        [Space(10f)]
        [Tooltip("Effects Executed when the Ability is activated.")]
        [SerializeField] private EffectBase[] _abilityEffects;
        
        [Space(10f)]
        [SerializeField] protected List<EventEffect> _animationEventEffects;
        
        public EventEffect[] EventEffects => _animationEventEffects.ToArray();

        public float Cooldown => _cooldown;
        
        public string AbilityName => _abilityName;
        
        public ResourceCost[] ResourceCosts => _resourceCosts;

        public EffectBase[] AbilityEffects => _abilityEffects;

        public bool CanBeInterrupted => _canBeInterrupted;

        public float AbilityDuration => _abilityDuration;

        public bool SetTrigger => _setTrigger;

        public string TriggerName => _triggerName;

        private void OnValidate()
        {
            if (EventEffects == null) return;
            
            for (int i = 0; i < EventEffects.Length; i++)
            {
                EventEffects[i].EventName = EventEffects[i].Event != null ?
                    EventEffects[i].Event.EventName : string.Empty;
            }
        }

        public void InsertAbilityEffect(EffectBase effect)
        {
            // Append effect to AbilityEffects array
            
            var list = new List<EffectBase>(_abilityEffects) { effect };

            _abilityEffects = list.ToArray();
        }
        
        public void DeleteAbilityEffect(EffectBase effect)
        {
            // Remove effect from AbilityEffects array
            
            var list = new List<EffectBase>(_abilityEffects);
            list.Remove(effect);

            _abilityEffects = list.ToArray();
        }
    }
}