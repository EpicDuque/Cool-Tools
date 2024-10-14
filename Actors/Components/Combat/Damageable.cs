using System;
using CoolTools.Attributes;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Actors
{
    public class Damageable : OwnableBehaviour, IDamageable
    {
        [ColorSpacer("Damageable")]
        [SerializeField] private ActorFaction faction;

        [Header("Vitals")]
        [SerializeField] private ActorFormulaEvaluator evaluator;
        
        [Space(5f)]
        [SerializeField] private Formula formulaMaxHealth;
        [SerializeField] private int maxHealth;
        
        [Space(5f)]
        [SerializeField] private Formula formulaHealthRegen;
        [SerializeField] private int healthRegen;

        [Space(5f)]
        [SerializeField] private int health;
        [SerializeField] private bool invincible;

        [Header("Extra Settings")] 
        [SerializeField] private DamageMultipliers[] multipliers;
        
        [ColorSpacer("Events")] 
        [SerializeField] private DamageableEvents damageableEvents;

        private IDisposable regenDisposable;

        [Serializable]
        public struct DamageableEvents
        {
            public UnityEvent<int> onDamage;
            public UnityEvent<int> onCritical;
            public UnityEvent<HitBox> onHit;
            public UnityEvent onDeath;
            public UnityEvent onRevive;
        }

        [Serializable]
        public struct DamageMultipliers
        {
            public DamageType DamageType;
            public Formula MultiplierFormula;
            public float Multiplier;
        }

        [field: SerializeField, InspectorDisabled] public Actor LastAttacker { get; set; }

        public UnityEvent<int> OnDamage => damageableEvents.onDamage;
        public UnityEvent<int> OnCritical => damageableEvents.onCritical;
        public UnityEvent<HitBox> OnHit => damageableEvents.onHit;
        public UnityEvent OnDeath => damageableEvents.onDeath;
        public UnityEvent OnRevive => damageableEvents.onRevive;

        public event Action HealthUpdate;

        public int MaxHealth
        {
            get => maxHealth;
            set => maxHealth = value;
        }

        public int HealthRegen
        {
            get => healthRegen;
            set => healthRegen = value;
        }

        public DamageMultipliers[] Multipliers
        {
            get => multipliers;
            set => multipliers = value;
        }

        public bool Invincible
        {
            get => invincible;
            set => invincible = value;
        }

        public ActorFaction Faction
        {
            get => faction;
            set => faction = value;
        }

        public bool IsAlive { get; protected set; } = true;

        /// <summary>
        /// Current health of the Damageable. This property can be modified directly but no other logic or events
        /// asociated with damage will occur.
        /// </summary>
        public virtual int Health
        {
            get => health;
            set
            {
                health = Mathf.Clamp(value, 0, maxHealth);
                HealthUpdate?.Invoke();
                
                if (health > 0)
                {
                    if (!IsAlive)
                    {
                        IsAlive = true;
                        OnRevive?.Invoke();
                    }
                    
                    return;
                }

                if (!IsAlive) return;
                
                health = 0;
                IsAlive = false;

                OnDeath?.Invoke();
            }
        }

        protected void OnValidate()
        {
            if (Application.isPlaying) return;

            EvaluateAll();
            
            if(HasOwner)
                faction = Owner.Faction;
        }

        protected override void OnOwnerChanged(Actor newOwner)
        {
            base.OnOwnerChanged(newOwner);

            IsAlive = true;
        }
        
        private void Start()
        {
            if (!HasOwner)
            {
                EvaluateAll();
                RestoreHealth();
            
                IsAlive = true;
            }
        }

        public void EvaluateAll()
        {
            if (evaluator == null) return;

            maxHealth = Mathf.RoundToInt(evaluator.Evaluate(formulaMaxHealth, Owner, maxHealth));
            healthRegen = Mathf.RoundToInt(evaluator.Evaluate(formulaHealthRegen, Owner, healthRegen));
            
            EvaluateAllMultipliers();
        }

        private void EvaluateAllMultipliers()
        {
            if (!HasOwner) return;

            for (var index = 0; index < multipliers.Length; index++)
            {
                var mult = multipliers[index];
                if(mult.MultiplierFormula == null) continue;
                
                mult.Multiplier = evaluator.Evaluate(mult.MultiplierFormula, Owner);
                multipliers[index] = mult;
            }
        }

        protected override void OnStatsUpdated()
        {
            EvaluateAll();
        }

        /// <summary>
        /// Restore health to max health. This can revive the Damageable (IsAlive will be true).
        /// </summary>
        [ContextMenu("Restore")]
        public void RestoreHealth()
        {
            Health = MaxHealth;
        }

        /// <summary>
        /// Deals damage on this Damageable.
        /// </summary>
        /// <param name="amt">Amount to reduce. Negative values mean healing.</param>
        /// <param name="type">Damage Type applied.</param>
        /// <param name="source">Actor source. Used to set who dealt damage last on this Damageable.</param>
        /// <param name="critical">If the damage reported counts as critial hit or not.</param>
        public virtual void DealDamage(int amt, DamageType type, OwnableBehaviour source = null, bool critical = false)
        {
            if (!IsAlive || Invincible) return;

            if(source != null)
                LastAttacker = source.Owner;

            var mult = 1f;

            // Apply DamageType Multiplier
            if (type != DamageType.None)
            {
                var damageMult = GetMultiplier(type);
                
                mult = damageMult.Multiplier;
            }

            amt = Mathf.RoundToInt(amt * mult);
            
            // TODO: Add Damage reduction here
            
            Health -= Mathf.RoundToInt(amt);

            if (amt != 0)
            {
                if (critical)
                    OnCritical?.Invoke(amt);
                else
                    OnDamage?.Invoke(amt);
            }
            
            if (source == null) return;
            if (source is not HitBox hitbox) return;
            
            if (Health > 0)
                OnHit?.Invoke(hitbox);
        }

        private DamageMultipliers GetMultiplier(DamageType type)
        {
            var damageMult = new DamageMultipliers();
            foreach (var m in multipliers)
            {
                if (m.DamageType != type) continue;
                damageMult = m;
                break;
            }

            return damageMult;
        }

        [ContextMenu("Kill")]
        public void Kill() => Health = 0;

        private void UpdateRegenInterval()
        {
            regenDisposable?.Dispose();
            if (healthRegen <= 0) return;
            
            regenDisposable = Observable.Interval(TimeSpan.FromSeconds(1f / healthRegen))
                .Subscribe(_ => Health++).AddTo(this);
        }
    }
}
