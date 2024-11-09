using CoolTools.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Actors
{
    public class ResourceContainer : OwnableBehaviour
    {
        [ColorSpacer("Resource Container")]
        [SerializeField] private ActorResource _resource;
        
        [Space(5f)]
        [SerializeField] private IntValueConfig _maxAmount;
        
        [Space(5f)] 
        [SerializeField] private FloatValueConfig _regenRate;

        [Space(10f)]
        [SerializeField] protected int _amount;
        [SerializeField] private bool _initializeAmount;
        

        [Space(10f)] 
        public UnityEvent<int> AmountChanged;

        public ActorResource Resource
        {
            get => _resource;
            set => _resource = value;
        }

        public IntValueConfig MaxAmount => _maxAmount;

        public virtual int Amount
        {
            get => _amount;
            set
            {
                _amount = Mathf.Clamp(value, 0, MaxAmount.Value);
                
                AmountChanged?.Invoke(_amount);
            }
        }
        
        public float Percent => (float) Amount / MaxAmount.Value;

        public float RegenRate
        {
            get => _regenRate.Value;
            set => _regenRate.BaseValue = value;
        }
        
        private void OnValidate()
        {
            _maxAmount.UpdateValue(this);
            _regenRate.UpdateValue(this);
            
            if(_amount > MaxAmount.Value)
                _amount = MaxAmount.Value;
        }
        
        private void OnEnable()
        {
            Amount = _amount;
        }

        private void Start()
        {
            if(_initializeAmount)
                Restore();
        }

        protected override void OnStatsUpdated()
        {
            _regenRate.UpdateValue(Owner);
            
            var oldPercent = Percent;
            _maxAmount.UpdateValue(Owner);
            
            if (oldPercent > 0f)
                Amount = Mathf.CeilToInt(MaxAmount.Value * oldPercent);
        }

        public virtual void Restore()
        {
            Amount = MaxAmount.Value;
        }
    }
}