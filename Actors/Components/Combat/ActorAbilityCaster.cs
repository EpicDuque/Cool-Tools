using System;
using System.Linq;
using CoolTools.Attributes;
using CoolTools.Data;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using Observable = UniRx.Observable;

namespace CoolTools.Actors
{
    public class ActorAbilityCaster : OwnableBehaviour
    {
        [Serializable]
        public class CombatEvents
        {
            [SerializeField] private UnityEvent<AbilityBase> onActorCastAbility;
            [SerializeField] private UnityEvent<AbilityBase> onActorEndCast;
            [SerializeField] private UnityEvent<AbilityBase> onActorCastInterrupt;
            
            public UnityEvent<AbilityBase> OnActorCastAbility => onActorCastAbility;
            public UnityEvent<AbilityBase> OnActorEndCast => onActorEndCast;
            public UnityEvent<AbilityBase> OnActorCastInterrupt => onActorCastInterrupt;
        }

        [ColorSpacer("Ability Caster")] 
        [SerializeField] private AbilityBase defaultAbility;
        
        [Space(10f)] 
        [SerializeField] private ActorFormulaEvaluator evaluator;
        [SerializeField] private ActorAnimationEventListener eventListener;

        [Space(10f)]
        [Tooltip("Resource containers to use for Ability resource costs")]
        [SerializeField] private ActorResourceContainer[] resourceContainers;

        [Space(10f)]
        [SerializeField] protected CombatEvents actorCombatEvents;
        
        [field: SerializeField, InspectorDisabled]
        public float Cooldown { get; protected set; }
        [SerializeField, InspectorDisabled] private float castingAbilityCooldown;

        [SerializeField] private FloatVariable cooldownVariable;

        public CombatEvents ActorCombatEvents => actorCombatEvents;
        public bool IsAiming { get; protected set; }
        public bool Casting { get; protected set; }
        [field: SerializeField, InspectorDisabled] public AbilityBase CastingAbility { get; set; }
        public bool IsReady => Cooldown <= 0f || !Casting;

        private AbilityBase lastAbility;
        private bool forceEvaluate;
        private float castingAbilityDuration;
        
        private void OnEnable()
        {
            if(eventListener != null)
                eventListener.OnEvent.AddListener(OnAbilityEvent);
        }

        private void OnDisable()
        {
            if(eventListener != null)
                eventListener.OnEvent.RemoveListener(OnAbilityEvent);
        }

        protected override void OnStatsUpdated()
        {
            base.OnStatsUpdated();

            forceEvaluate = true;
        }

        private void OnAbilityEvent(AnimationEventSO animEvent)
        {
            if (CastingAbility == null) return;

            if (animEvent.EventName != "End")
            {
                var ev = CastingAbility.EventEffects
                    .FirstOrDefault(ef => ef.Event == animEvent);

                if (ev.Event == null) return;
                
                ev.Effect.ExecuteEffect(Owner);
            }
            else if(Casting)
            {
                EndAbilityCast();
            }
        }

        public void CastAbility(AbilityBase ability = null)
        {
            if(Cooldown > 0f || Casting) return;

            CastingAbility = ability != null ? ability : defaultAbility;
            
            if (CastingAbility != lastAbility || forceEvaluate)
            {
                lastAbility = CastingAbility;
                castingAbilityCooldown = evaluator.Evaluate(CastingAbility.FormulaCooldown, Owner);
                castingAbilityDuration = evaluator.Evaluate(CastingAbility.FormulaAbilityDuration, Owner);
            }

            if (CastingAbility == null) return;
            
            if (!HasResourcesForAbility(CastingAbility))
            {
                Debug.Log($"[{nameof(ActorAbilityCaster)}] tried to cast an Ability without the required resources");
                return;
            }
            
            foreach (var rc in CastingAbility.ResourceCosts)
            {
                var container = resourceContainers.FirstOrDefault(cnt => cnt.Resource == rc.Resource);
            
                if (container == null)
                {
                    Debug.LogError($"[{nameof(ActorAbilityCaster)}] Something went wrong here, there is supposed to be a valid container...");
                    return;
                }
                
                container.Amount -= rc.Amount;
            }
            
            foreach (var casterEffect in CastingAbility.CasterEffects)
            {
                casterEffect.ExecuteEffect(Owner);    
            }

            Casting = true;
            ActorCombatEvents.OnActorCastAbility?.Invoke(CastingAbility);

            if (!CastingAbility.UseAnimEvents)
            {
                Observable.Timer(TimeSpan.FromSeconds(castingAbilityDuration))
                    .Where(_ => Casting)
                    .Subscribe(_ => EndAbilityCast()).AddTo(this);
            }
        }
        
        public bool HasResourcesForAbility(AbilityBase ability)
        {
            return ability.ResourceCosts.Length == 0 || ability.ResourceCosts.All(rc =>
                resourceContainers.Any(c => c.Resource == rc.Resource && c.Amount >= rc.Amount));
        }

        private void EndAbilityCast(bool startCooldown = true)
        {
            if (Casting || CastingAbility != null)
            {
                ActorCombatEvents.OnActorEndCast?.Invoke(CastingAbility);
            }
            
            Casting = false;
            
            Cooldown = startCooldown ? castingAbilityCooldown : 0f;
            
            CastingAbility = null;
            Casting = false;
        }

        public void InterruptAbility(bool startCooldown = true)
        {
            if (CastingAbility != null && Casting)
            {
                foreach (var e in CastingAbility.EventEffects.Select(ef => ef.Effect))
                    e.ResetEffect(Owner);
                
                ActorCombatEvents.OnActorCastInterrupt?.Invoke(CastingAbility);
            }
            
            EndAbilityCast(startCooldown);
        }

        private void Update()
        {
            if (Cooldown > 0f)
                Cooldown -= Time.deltaTime;
            else Cooldown = 0f;
            
            if(cooldownVariable != null)
                cooldownVariable.Value = Cooldown;
        }
    }
}
