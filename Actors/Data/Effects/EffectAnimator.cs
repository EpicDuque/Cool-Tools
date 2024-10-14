using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New Animator Effect", menuName = "Effect/Animator")]
    public class EffectAnimator : EffectBase
    {
        public enum TriggerAction
        {
            Set, Reset,    
        }
        
        [Header("Trigger")]
        [SerializeField] private TriggerAction triggerAction;
        [SerializeField] private string triggerName;
        
        [Header("Boolean")]
        [SerializeField] private string booleanName;
        [SerializeField] private bool boolState;
        
        [Header("Float")]
        [SerializeField] private string floatName;
        [SerializeField] private float floatValue;
        
        [Header("Integer")]
        [SerializeField] private string intName;
        [SerializeField] private int intValue;
        
        [FormerlySerializedAs("pauseAnimator")]
        [Space(10f)] 
        [SerializeField] private bool disableAnimator;

        public override void ExecuteEffect(Actor target, Actor source = null)
        {
            base.ExecuteEffect(target, source);

            if (!target.HasAnimator) return;
            
            var animator = target.Animator;

            if (!string.IsNullOrEmpty(triggerName))
            {
                switch (triggerAction)
                {
                    case TriggerAction.Set:
                        animator.SetTrigger(triggerName);
                        break;
                    
                    case TriggerAction.Reset:
                        animator.ResetTrigger(triggerName);
                        break;
                    
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (!string.IsNullOrEmpty(booleanName))
            {
                animator.SetBool(booleanName, boolState);
            }

            if (!string.IsNullOrEmpty(floatName))
            {
                animator.SetFloat(floatName, floatValue);
            }

            if (!string.IsNullOrEmpty(intName))
            {
                animator.SetInteger(intName, intValue);
            }
            
            if (disableAnimator)
            {
                animator.enabled = false;
            }
        }

        public override void ResetEffect(Actor target)
        {
            base.ResetEffect(target);
            
            if (!target.HasAnimator) return;
            
            var animator = target.Animator;

            if (disableAnimator)
                animator.enabled = true;
        }
    }
}