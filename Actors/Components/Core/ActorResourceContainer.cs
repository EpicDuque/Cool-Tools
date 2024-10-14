using System;
using CoolTools.Data;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Actors
{
    public class ActorResourceContainer : OwnableBehaviour
    {
        [Space(10f)]
        [SerializeField] private ActorResource resource;
        
        [Space(10f)] 
        [SerializeField] private ActorFormulaEvaluator evaluator;
        
        [Space(5f)]
        [SerializeField] private Formula formulaMaxAmount;
        [SerializeField] private int maxAmount;
        
        [Space(5f)]
        [SerializeField] private Formula formulaRegenRate;
        [SerializeField] private float regenRate;
        
        [Space(5f)]
        [SerializeField] private Formula formulaRegenAmount;
        [SerializeField] private int regenAmount;
        
        [Space(10f)]
        [SerializeField] private bool regenWhileDepleted;
        [SerializeField] private bool restoreOnStart = true;

        [Space(10f)] 
        [SerializeField] private IntVariable resourceAmountVariable;
        [SerializeField] private FloatVariable resourcePctVariable;

        [Space(10f)]
        [SerializeField] private int amount;

        [Space(10f)] 
        public UnityEvent AmountChanged;

        private bool hasResourceAmountVar;
        private bool hasResourcePctVar;
        
        public ActorResource Resource
        {
            get => resource;
            set => resource = value;
        }

        public int MaxAmount
        {
            get => maxAmount;
            set => maxAmount = value;
        }

        public int Amount
        {
            get => amount;
            set
            {
                amount = Mathf.Clamp(value, 0, MaxAmount);
                UpdateVariables();
                AmountChanged?.Invoke();
            }
        }

        private IDisposable regenDisposable;

        private void OnValidate()
        {
            if (Owner == null) return;
            
            OnStatsUpdated();
            UpdateVariables();
        }
        
        private void OnEnable()
        {
            hasResourceAmountVar = resourceAmountVariable != null;
            hasResourcePctVar = resourcePctVariable != null;

            Amount = amount;
            UpdateRegenInterval();
        }

        private void Start()
        {
            if(restoreOnStart)
                Restore();
        }

        protected override void OnOwnerChanged(Actor newOwner)
        {
            base.OnOwnerChanged(newOwner);
            if (!HasOwner) return;
            
            OnStatsUpdated();
        }

        protected override void OnStatsUpdated()
        {
            if (!HasOwner) return;
            
            regenRate = evaluator.Evaluate(formulaRegenRate, Owner, regenRate);
            maxAmount = Mathf.RoundToInt(evaluator.Evaluate(formulaMaxAmount, Owner, maxAmount));
            regenAmount = Mathf.RoundToInt(evaluator.Evaluate(formulaRegenAmount, Owner, regenAmount));
            
            UpdateRegenInterval();
        }
        
        private void UpdateRegenInterval()
        {
            if (!Application.isPlaying || !HasOwner) return;
            
            if (!gameObject.activeInHierarchy) return;
            
            regenDisposable?.Dispose();
            regenDisposable = Observable.Interval(TimeSpan.FromSeconds(1f / regenRate))
                .Subscribe(_ => Amount += regenAmount).AddTo(this);
        }

        private void OnDisable()
        {
            regenDisposable?.Dispose();
        }

        private void UpdateVariables()
        {
            if (hasResourceAmountVar)
                resourceAmountVariable.Value = amount;

            if (hasResourcePctVar)
                resourcePctVariable.Value = (amount * 100f / maxAmount);
        }

        public void Restore()
        {
            Amount = MaxAmount;
        }
    }
}