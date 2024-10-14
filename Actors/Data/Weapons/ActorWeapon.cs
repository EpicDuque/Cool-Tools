using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Actor/Weapon", order = 0)]
    public class ActorWeapon : ScriptableObject
    {
        [SerializeField] protected string weaponName;
        [SerializeField] protected WeaponModel prefabModel;
        
        [Space(10f)]
        [SerializeField] protected EffectBase[] equipEffects;

        [Space(10f)]
        [SerializeField] private AbilityBase[] abilitySequence;

        public string WeaponName => weaponName;
        public WeaponModel PrefabModel => prefabModel;
        public AbilityBase[] AbilitySequence => abilitySequence;
        public EffectBase[] EquipEffects => equipEffects;

    }
}
