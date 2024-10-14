using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New Effect Toggle", menuName = "Effect/Toggle Active")]
    public class EffectToggleActive : EffectBase
    {
        [SerializeField] private bool state;
        [SerializeField] private bool resetState;
        [SerializeField] private List<EffectTargetTag> tags;

        public override void ExecuteEffect(Actor target, Actor source = null)
        {
            base.ExecuteEffect(target, source);

            var objects = target.FindAllEffectTargets(tags).ToArray();

            foreach (var o in objects)
            {
                o.gameObject.SetActive(state);
            }
        }

        public override void ResetEffect(Actor target)
        {
            base.ResetEffect(target);
            
            var objects = target.FindAllEffectTargets(tags).ToArray();

            foreach (var o in objects)
            {
                o.gameObject.SetActive(resetState);
            }
        }
    }
}