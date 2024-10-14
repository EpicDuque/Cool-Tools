using UnityEngine;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New Damageable Effect", menuName = "Effect/Damageable", order = 0)]
    public class EffectDamageable : EffectBase
    {
        [HelpBox("Negative amounts mean healing.")]
        [SerializeField] private int damageAmount;
        [SerializeField] private DamageType damageType;

        [Space(10f)]
        [SerializeField] private float percent;
        
        [Space(10f)] 
        [SerializeField] private bool changeInvincibility;
        [SerializeField] private bool invincibilityState;

        public override void ExecuteEffect(Actor target, Actor source = null)
        {
            base.ExecuteEffect(target, source);
            
            if(damageAmount != 0)
                target.Damageable.DealDamage(damageAmount, damageType);
            else
            {
                var amount = target.Damageable.MaxHealth * percent;
                target.Damageable.DealDamage(Mathf.RoundToInt(amount), damageType);
            }
            
            if(changeInvincibility)
                target.Damageable.Invincible = invincibilityState;
        }

        public override void ResetEffect(Actor target)
        {
            base.ResetEffect(target);
            
            if(changeInvincibility)
                target.Damageable.Invincible = !invincibilityState;
        }
    }
}