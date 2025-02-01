using System;
using System.Collections.Generic;
using CoolTools.Attributes;
using UnityEngine;
using UnityEngine.Events;

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
        [SerializeField] private bool _oncePerTarget;
        [SerializeField] private bool _hitInvincibles = true;
        
        [Space(10f)]
        public HitEvents Events;
        
        [Space(10f)] [SerializeField]
        private FactionOperations.FactionFilterMode _factionFilter = FactionOperations.FactionFilterMode.NotOwner;
        
        private List<IDamageable> _damageablesHit = new();
        private List<IDamageable> _insideDamageables = new();
        private Collider _hitBoxCollider;
        
        public FactionOperations.FactionFilterMode FactionFilter => _factionFilter;
        public Collider HBCollider => _hitBoxCollider;
        public IDamageable[] InsideDamageables => _insideDamageables.ToArray();

        public IntValueConfig Power
        {
            get => _power;
            set => _power = value;
        }

        protected void OnValidate()
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
            
            if (!_hitInvincibles && damageable.Invincible) return;
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
            
            Hit(damageable, other.ClosestPoint(transform.position));
        }

        private void OnTriggerExit(Collider other)
        {
            if (!enabled) return;
            if (!other.TryGetComponent<IDamageable>(out var damageable)) return;
            
            if(_insideDamageables.Contains(damageable))
                _insideDamageables.Remove(damageable);
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