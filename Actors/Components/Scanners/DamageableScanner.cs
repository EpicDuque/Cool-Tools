using System.Linq;
using UnityEngine;

namespace CoolTools.Actors
{
    public class DamageableScanner : Scanner<Damageable>
    {
        [SerializeField] private bool aliveOnly = true;
        [SerializeField] private bool invulnerables;
        
        [Space(10f)] 
        [SerializeField] private FactionOperations.FactionFilterMode factionFilterMode = FactionOperations.FactionFilterMode.Include;
        [SerializeField] private ActorFaction[] factions;

        protected override bool IsTargetValid(Damageable component)
        {
            return (component.IsAlive && aliveOnly || !aliveOnly) &&
                   (component.Invincible && invulnerables || !component.Invincible && !invulnerables) && 
                   FactionOperations.IsValidFaction(component.Faction, factions, factionFilterMode);
        }

        protected override bool SelfCheck(Damageable other)
        {
            return other.Owner == Owner;
        }

        protected override void OnObjectAdded(Damageable obj)
        {
            base.OnObjectAdded(obj);
            
            obj.OnDeath.AddListener(() =>
            {
                objectColliders.Remove(obj);
                nearObjects.Remove(obj);
            });
        }
    }
}