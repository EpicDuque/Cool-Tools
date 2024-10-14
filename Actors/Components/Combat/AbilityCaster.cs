using System;
using System.Collections;
using System.Linq;
using CoolTools.Attributes;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Actors
{
    public class AbilityCaster : OwnableBehaviour
    {
        [Serializable]
        public struct Events
        {
            public UnityEvent<AbilityBase> OnActorCastAbility;
            public UnityEvent<AbilityBase> OnActorEndCast;
            public UnityEvent<AbilityBase> OnActorCastInterrupt;
        }
        
        [ColorSpacer("Ability Caster")]
        [SerializeField] protected AbilityBase _castingAbility;
        [SerializeField] protected FloatValueConfig _durationMultiplier;
        [SerializeField] protected FloatValueConfig _cooldownMultiplier;
        
        [Space(10f)] 
        [SerializeField] protected ActorAnimationEventListener _eventListener;
        
        [Space(10f)]
        [Tooltip("Resource containers to use for Ability resource costs")]
        [SerializeField] protected ResourceContainer[] _resourceContainers;

        [Space(10f)]
        [SerializeField] protected Events _actorCombatEvents;
        
        [Header("Debug")]
        [SerializeField, InspectorDisabled] protected Transform _castTargetPoint;
        [SerializeField, InspectorDisabled] protected Vector3 _castPosition;
        [SerializeField, InspectorDisabled] protected CasterState _state;
        
        protected IDetectable _castTarget;
        protected Actor _castTargetActor;
        // protected Subject<AbilityBase> _endAbilitySubject = new ();
        private bool _abilityEndSignal;
        protected Subject<AbilityBase> _endEffectSequence = new ();
        
        [field: SerializeField, InspectorDisabled]
        public float Cooldown { get; protected set; }

        public CasterState State
        {
            get => _state;
            protected set => _state = value;
        }

        public AbilityBase CastingAbility
        {
            get => _castingAbility;
            set => _castingAbility = value;
        }
        
        public Events ActorCombatEvents => _actorCombatEvents;
        
        public bool IsReady => Cooldown <= 0f && State != CasterState.Casting;

        public ActorAnimationEventListener EventListener
        {
            get => _eventListener;
            set
            {
                if(_eventListener != null)
                    _eventListener.OnEvent.RemoveListener(OnAbilityEvent);
                
                _eventListener = value;
                
                if(_eventListener != null)
                    _eventListener.OnEvent.AddListener(OnAbilityEvent);
            }
        }
        
        [Serializable]
        public enum CasterState
        {
            Ready,
            Casting,
            Cooldown,
        }

        private Coroutine _castingRoutine;

        protected new void Reset()
        {
            base.Reset();

            _cooldownMultiplier.Multiplier = 1;
            _durationMultiplier.Multiplier = 1;

            _cooldownMultiplier.BaseValue = 1;
            _durationMultiplier.BaseValue = 1;
            
            _cooldownMultiplier.UpdateValue();
            _durationMultiplier.UpdateValue();
        }

        private void OnValidate()
        {
            if (Owner != null)
            {
                _cooldownMultiplier.UpdateValue(Owner);
                _durationMultiplier.UpdateValue(Owner);
            }
            else
            {
                _cooldownMultiplier.UpdateValue();
                _durationMultiplier.UpdateValue();
            }
        }

        private void OnEnable()
        {
            if(_eventListener != null)
                _eventListener.OnEvent.AddListener(OnAbilityEvent);
        }

        private void OnDisable()
        {
            if(_eventListener != null)
                _eventListener.OnEvent.RemoveListener(OnAbilityEvent);
        }

        protected new void Awake()
        {
            base.Awake();
            
            ResetCaster();
        }

        protected override void OnStatsUpdated()
        {
            base.OnStatsUpdated();
            
            var eparams = new EvaluateParams
            {
                Source = Owner,
            };
            
            _cooldownMultiplier.UpdateValue(Owner.Evaluator, eparams);
            _durationMultiplier.UpdateValue(Owner.Evaluator, eparams);
        }

        private void OnAbilityEvent(AnimationEventSO animEvent)
        {
            if (State != CasterState.Casting) return;

            if (!animEvent.EndsAbility)
            {
                var ev = _castingAbility.EventEffects
                    .FirstOrDefault(ef => ef.Event == animEvent);

                if (ev.Event == null) return;
                
                // if(Owner.name.Contains("Player"))
                //     Debug.Log($"[{nameof(AbilityCaster)}] Found Event: {ev.Event.EventName} ({ev.Event.name})");

                // Do The event here
                foreach (var effect in ev.Effects)
                {
                    // TODO: Check if we are casting with target or position so we execute these with target or position as well.
                    effect.Execute(Owner);
                }
            }
            else
            {
                _abilityEndSignal = true;
            }
        }
        
        #region CastAbility
        
        private IEnumerator CastingRoutine(IEnumerator effectExecuteRoutine)
        {
            State = CasterState.Casting;

            StartCoroutine(effectExecuteRoutine);

            if (_castingAbility.AbilityDuration > 0f)
            {
                var duration = _castingAbility.AbilityDuration * _durationMultiplier.Value;
                yield return new WaitForSeconds(duration);
            }
            else
            {
                _abilityEndSignal = false;
                while (!_abilityEndSignal)
                    yield return null;
            }
            
            EndAbility();

            State = CasterState.Cooldown;
            
            // Apply cooldown multipliers here
            yield return new WaitUntil(() => Cooldown <= 0f);
            
            Cooldown = 0f;
            State = CasterState.Ready;
            
            _castingRoutine = null;
            _abilityEndSignal = false;
        }
        
        private IEnumerator ExecuteEffectsRoutine()
        {
            foreach(var effect in _castingAbility.AbilityEffects)
            {
                effect.Execute(Owner);
                if (effect is IEffectWaiter waiter)
                {
                    yield return waiter.Wait;
                }
            }
            
            _endEffectSequence.OnNext(_castingAbility);
        }
        
        private IEnumerator ExecuteEffectsRoutine(Vector3 position)
        {
            foreach(var effect in _castingAbility.AbilityEffects)
            {
                effect.Execute(Owner, position);
                if (effect is IEffectWaiter waiter)
                {
                    yield return waiter.Wait;
                }
            }
            
            _endEffectSequence.OnNext(_castingAbility);
        }
        
        private IEnumerator ExecuteEffectsRoutine(IDetectable target)
        {
            foreach(var effect in _castingAbility.AbilityEffects)
            {
                effect.Execute(Owner, target);
                if (effect is IEffectWaiter waiter)
                {
                    yield return waiter.Wait;
                }
            }
            
            _endEffectSequence.OnNext(_castingAbility);
        }
        
        private IEnumerator ExecuteEffectsRoutine(Actor target)
        {
            foreach(var effect in _castingAbility.AbilityEffects)
            {
                effect.Execute(Owner, target);
                if (effect is IEffectWaiter waiter)
                {
                    yield return waiter.Wait;
                }
            }
            
            _endEffectSequence.OnNext(_castingAbility);
        }
        
        public void CastAbility()
        {
            if (!IsReadyToCast()) return;
            
            State = CasterState.Casting;
            
            _castingRoutine = StartCoroutine(CastingRoutine(ExecuteEffectsRoutine()));
            ActorCombatEvents.OnActorCastAbility?.Invoke(_castingAbility);
            
            TriggerAnimatorFields();
        }

        public void CastAbility(Vector3 position)
        {
            if (!IsReadyToCast()) return;
            
            State = CasterState.Casting;
            _castPosition = position;
            
            _castingRoutine = StartCoroutine(CastingRoutine(ExecuteEffectsRoutine(position)));
            ActorCombatEvents.OnActorCastAbility?.Invoke(_castingAbility);
            
            TriggerAnimatorFields();
        }

        public void CastAbility(IDetectable target)
        {
            if (!IsReadyToCast()) return;

            State = CasterState.Casting;
            
            _castTarget = target;
            _castTargetPoint = target.TargetPoint;
            // Cast the ability with target transform here
            
            _castingRoutine = StartCoroutine(CastingRoutine(ExecuteEffectsRoutine(target)));
            ActorCombatEvents.OnActorCastAbility?.Invoke(_castingAbility);
            
            TriggerAnimatorFields();
        }
        
        public void CastAbility(Actor target)
        {
            if (!IsReadyToCast()) return;

            State = CasterState.Casting;
            
            _castTargetActor = target;
            _castTargetPoint = target.transform;
            // Cast the ability with target transform here
            
            _castingRoutine = StartCoroutine(CastingRoutine(ExecuteEffectsRoutine(target)));
            ActorCombatEvents.OnActorCastAbility?.Invoke(_castingAbility);
            
            TriggerAnimatorFields();
        }
        
        private void TriggerAnimatorFields()
        {
            if(_castingAbility.SetTrigger && Owner.HasAnimator)
                Owner.Animator.SetTrigger(_castingAbility.TriggerName);
        }

        #endregion
        
        public void ResetCaster()
        {
            StopAllCoroutines();
            Cooldown = 0f;
            State = CasterState.Ready;
        }
        
        public bool IsReadyToCast()
        {
            if (Cooldown > 0f || State != CasterState.Ready) return false;
            
            if (_castingAbility == null) return false;
            
            if (!HasResourcesForAbility(_castingAbility))
            {
                Debug.Log($"[{nameof(AbilityCaster)}] tried to cast an Ability without the required resources");
                return false;
            }
            
            return true;
        }

        public bool HasResourcesForAbility(AbilityBase ability)
        {
            return ability.ResourceCosts.Length == 0 || ability.ResourceCosts.All(rc =>
                _resourceContainers.Any(c => c.Resource == rc.Resource && c.Amount >= rc.Amount));
        }

        private void EndAbility(float cooldown = -1f)
        {
            if (State == CasterState.Casting || _castingAbility != null)
            {
                ActorCombatEvents.OnActorEndCast?.Invoke(_castingAbility);
            }

            Cooldown = _castingAbility.Cooldown * _cooldownMultiplier.Value;
        }

        public void InterruptAbility()
        {
            if (State != CasterState.Casting) return;

            _abilityEndSignal = true;
        }

        private void Update()
        {
            if (State == CasterState.Cooldown)
            {
                if (Cooldown > 0f)
                    Cooldown -= Time.deltaTime;
                else
                {
                    Cooldown = 0f;
                    State = CasterState.Ready;
                }
            }
        }

        public void ForceCooldown(float cooldown)
        {
            if (State == CasterState.Casting)
            {
                StopAllCoroutines();
                
                EndAbility();

                State = CasterState.Cooldown;
                Cooldown = cooldown;
                _castingRoutine = null;
            }
            else
            {
                State = CasterState.Cooldown;
                Cooldown = cooldown;
            }
        }
    }
}
