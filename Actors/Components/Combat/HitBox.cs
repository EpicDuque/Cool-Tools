using System;
using System.Collections.Generic;
using CoolTools.Attributes;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace CoolTools.Actors
{
    public class HitBox : OwnableBehaviour
    {
        [Serializable]
        public struct HitEvents
        {
            public UnityEvent<IDamageable> OnHit;
            public UnityEvent<Vector3> OnHitPos;
            public UnityEvent<int> OnHitAmount;
        }
        
        [ColorSpacer("Hit Box")] 
        [SerializeField] private IntValueConfig _power;
        [SerializeField] private DamageType _damageType;
        [FormerlySerializedAs("_hitOnlyOnce")] 
        [SerializeField] private bool _oncePerTarget;
        
        [Header("Persistent Hits")]
        [SerializeField] private bool _persistentHits;
        [SerializeField] private float _persistentHitInterval;

        [Space(10f)]
        public HitEvents Events;
        
        [Space(10f)] [SerializeField]
        private FactionOperations.FactionFilterMode _factionFilter = FactionOperations.FactionFilterMode.NotOwner;
        
        private List<IDamageable> _damageablesHit = new();
        private List<IDamageable> _insideDamageables = new();
        private Dictionary<IDamageable, IDisposable> _persistentDamageables = new();
        private Collider _hitBoxCollider;
        
        // public ActorFaction[] Factions => _factions;
        public FactionOperations.FactionFilterMode FactionFilter => _factionFilter;

        public IntValueConfig Power
        {
            get => _power;
            set => _power = value;
        }

        private void OnValidate()
        {
            _power.UpdateValue(this);
        }

        protected new void Awake()
        {
            base.Awake();
            
            _hitBoxCollider = GetComponent<Collider>();
        }

        private void OnEnable()
        {
            _insideDamageables.Clear();
            _damageablesHit.Clear();
            
            _power.UpdateValue(this);
        }

        protected override void OnStatsUpdated()
        {
            base.OnStatsUpdated();
            
            _power.UpdateValue(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!enabled) return;
            if (!other.TryGetComponent<IDamageable>(out var damageable)) return;
            if (!damageable.IsAlive) return;
            if (_oncePerTarget && _damageablesHit.Contains(damageable)) return;

            _power.UpdateValue(this);
            if (HasOwner && damageable is IOwnable ownable && ownable.HasOwner)
            {
                if (_factionFilter == FactionOperations.FactionFilterMode.NotOwner)
                {
                    if(ownable.Owner.Faction == Owner.Faction) 
                        return;
                }
                else
                {
                    if(ownable.Owner.Faction != Owner.Faction) 
                        return;
                }
            }
            
            if(!_damageablesHit.Contains(damageable))
                _damageablesHit.Add(damageable);
            
            if(!_insideDamageables.Contains(damageable))
                _insideDamageables.Add(damageable);

            if (_persistentHits)
            {
                var observable = Observable.Interval(TimeSpan.FromSeconds(_persistentHitInterval)).Subscribe(_ =>
                {
                    // Faction check
                    if (damageable is IOwnable ownable)
                    {
                        if (_factionFilter == FactionOperations.FactionFilterMode.NotOwner)
                        {
                            if(ownable.Owner.Faction == Owner.Faction) 
                                return;
                        }
                        else
                        {
                            if(ownable.Owner.Faction != Owner.Faction) 
                                return;
                        }
                    }
                    
                    Hit(damageable, other.ClosestPoint(transform.position));
                }).AddTo(this);
                
                _persistentDamageables.Add(damageable, observable);
            }
            else
            {
                Hit(damageable, other.ClosestPoint(transform.position));
            }
        }

        public void Hit(IDamageable other, Vector3 hitPoint)
        {
            other.DealDamage(new DamageParams()
            {
                Amount = _power.Value,
                Source = Owner,
                Target = other,
                SourceObject = gameObject,
                Type = _damageType,
            });
            
            Events.OnHit?.Invoke(other);
            Events.OnHitPos?.Invoke(hitPoint);
            Events.OnHitAmount?.Invoke(_power.Value);
        }
    }
}