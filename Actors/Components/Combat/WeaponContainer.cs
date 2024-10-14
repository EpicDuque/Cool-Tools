using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoolTools.Actors
{
    public class WeaponContainer : OwnableBehaviour
    {
        [Space(10f)]
        [SerializeField] protected ActorWeapon weaponData;
        [Tooltip("Can I equip the same weapon that is already equipped?")]
        [SerializeField] protected bool canReEquip;

        [Tooltip("Ability caster used when the weapon is used.")]
        [SerializeField] protected ActorAbilityCaster abilityCaster;
        
        [Space(10f)]
        [Tooltip("Parent transform for the spawned weapon model.")]
        [SerializeField] protected Transform equipTransform;

        public WeaponModel SpawnedModel
        {
            get => spawnedModel;
            protected set
            {
                spawnedModel = value;
                HasModel = spawnedModel != null;
            }
        }
        
        public bool HasModel { get; protected set; }

        public ActorWeapon WeaponData
        {
            get => weaponData;
            protected set => weaponData = value;
        }
        
        private int sequenceLength = 0;
        private int sequenceIndex = 0;
        private Coroutine weaponUsageRoutine;
        private WeaponModel spawnedModel;

        public ActorAbilityCaster AbilityCaster => abilityCaster;

        private IEnumerator Start()
        {
            yield return null;
            
            UpdateWeaponModel();
        }

        private void OnDisable()
        {
            if(weaponUsageRoutine != null)
                StopCoroutine(weaponUsageRoutine);
        }

        public void SetWeaponData(ActorWeapon weapon)
        {
            if (!canReEquip && weapon == WeaponData) return;
            
            if (WeaponData != null && WeaponData.EquipEffects != null)
            {
                foreach(var ef in WeaponData.EquipEffects)
                    ef.ResetEffect(Owner);
            }

            WeaponData = weapon;
            
            UpdateWeaponModel();
            sequenceIndex = 0;
        }

        public void UpdateWeaponModel()
        {
            if (HasModel)
                Destroy(SpawnedModel.gameObject);
            
            if(WeaponData == null) return;
            
            if (WeaponData.PrefabModel != null && equipTransform != null)
            {
                spawnedModel = SpawnWithOwnership(WeaponData.PrefabModel, equipTransform);
                spawnedModel.WeaponData = WeaponData;
            }

            if (WeaponData.EquipEffects != null)
            {
                foreach(var ef in WeaponData.EquipEffects)
                {
                    ef.ExecuteEffect(Owner);
                }
            }
            
            if(spawnedModel != null)
                Owner.AddEffectTargets(spawnedModel.EffectTargets);

            sequenceLength = WeaponData.AbilitySequence.Length;
        }

        public void UseWeapon()
        {
            if(weaponUsageRoutine != null || WeaponData == null || abilityCaster.Casting || abilityCaster.Cooldown > 0f) return;

            weaponUsageRoutine = StartCoroutine(WeaponUsageRoutine());
        }

        private IEnumerator WeaponUsageRoutine()
        {
            if (WeaponData.AbilitySequence == null || sequenceLength == 0) yield break;
            
            var ability = WeaponData.AbilitySequence[sequenceIndex];

            if (ability == null) yield break;
            
            abilityCaster.CastAbility(ability);
            
            if(HasModel)
                spawnedModel.OnUseStart?.Invoke();

            sequenceIndex++;
            
            if (sequenceIndex >= sequenceLength)
                sequenceIndex = 0;

            yield return new WaitUntil(() => !abilityCaster.Casting && abilityCaster.Cooldown <= 0f);
            
            if(HasModel)
                spawnedModel.OnUseEnd?.Invoke();
            
            weaponUsageRoutine = null;
        }

        public void ResetAttack()
        {
            if(weaponUsageRoutine != null)
                StopCoroutine(weaponUsageRoutine);
            
            abilityCaster.InterruptAbility(false);
            sequenceIndex = 0;
            
            weaponUsageRoutine = null;
        }
    }
}
