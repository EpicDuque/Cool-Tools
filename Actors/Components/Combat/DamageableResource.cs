using System;
using CoolTools.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Actors
{
    public class DamageableResource : ResourceContainer, IDamageable
    {
        [ColorSpacer("Damageable Resource")] 
        [SerializeField] protected bool _invincible;
        
        [ColorSpacer("Events")]
        [SerializeField] protected DamageableEvents _damageableEvents;
        
        [Serializable]
        public struct DamageableEvents
        {
            public UnityEvent<int> OnDamage;
            public UnityEvent<int> OnHeal;
            
            public UnityEvent OnDeath;
            public UnityEvent OnRevive;
        }
        
        public DamageableEvents Events => _damageableEvents;

        [Serializable]
        public struct DamageMultiplier
        {
            [Space(10f)]
            public DamageType DamageType;
            
            [Space(5f)]
            public Formula FormulaMultiplier;
            
            [Space(5f)]
            public float Multiplier;
        }

        [field: SerializeField, InspectorDisabled] public DamageParams LastDamage { get; set; }

        public bool BypassDetection { get; set; } = false;
        
        public bool Invincible
        {
            get => _invincible;
            set => _invincible = value;
        }

        public override int Amount
        {
            get => _amount;
            set
            {
                var previousAmount = Amount;
                base.Amount = value;
                
                if (previousAmount > 0 && Amount <= 0)
                    Events.OnDeath?.Invoke();
            }
        }
        public GameObject GO => gameObject;
        public virtual bool IsAlive => Amount > 0;
        public int Health => Amount;
        public int MaxHealth => MaxAmount.Value;

        public virtual void DealDamage(DamageParams data)
        {
            if (!IsAlive) return;
            if (Invincible) return;

            LastDamage = data;
            Amount -= data.Amount;
            
            if(data.Amount < 0)
                Events.OnHeal?.Invoke(data.Amount);
            else if(data.Amount > 0)
                Events.OnDamage?.Invoke(data.Amount);
        }

        [ContextMenu("Kill")]
        public virtual void Kill()
        {
            var data = new DamageParams
            {
                Amount = MaxAmount.Value,
                Critical = false,
                Source = null,
            };
            DealDamage(data);
        }

        public void Revive()
        {
            if (IsAlive) return;
            
            Events.OnRevive?.Invoke();
            Restore();
        }
    }
}
