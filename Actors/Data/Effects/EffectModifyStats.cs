using UnityEngine;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New Modify Stats Effect", menuName = "Effect/Modify Stats")]
    public class EffectModifyStats : EffectBase
    {
        [SerializeField] private StatProvider.AttributeModification[] modifications;
        
        [Space(10f)] 
        [SerializeField] private bool _updateActorStats = true;

        public StatProvider.AttributeModification[] Modifications => modifications;

        public bool UpdateActorStats
        {
            get => _updateActorStats;
            set => _updateActorStats = value;
        }

        public override void ExecuteEffect(Actor target, Actor source = null)
        {
            base.ExecuteEffect(target, source);

            var provider = target.StatProvider;

            foreach (var mod in modifications)
            {
                var modification = provider.GetAttributeModification(mod.Attribute);
                if (modification.Attribute == null) continue;

                modification.Multiplier *= mod.Multiplier;
                modification.Offset += mod.Offset;
            }

            if(_updateActorStats)
                provider.UpdateActorCurrentStats();
        }

        public override void ResetEffect(Actor target)
        {
            base.ResetEffect(target);

            var provider = target.StatProvider;
            
            foreach (var mod in modifications)
            {
                var modification = provider.GetAttributeModification(mod.Attribute);
                if (modification.Attribute == null) continue;

                modification.Multiplier /= mod.Multiplier;
                modification.Offset -= mod.Offset;
            }
            
            if(_updateActorStats)
                provider.UpdateActorCurrentStats();
        }
    }
}