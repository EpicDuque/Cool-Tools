using UnityEngine;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New Exp Effect", menuName = "Effect/Experience")]
    public class EffectExperience : EffectBase
    {
        [SerializeField] private int amount;

        public override void ExecuteEffect(Actor target, Actor source = null)
        {
            base.ExecuteEffect(target, source);

            target.StatProvider.Experience += amount;
        }
    }
}