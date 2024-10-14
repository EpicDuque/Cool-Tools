using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Actors
{
    public class WeaponModel : OwnableBehaviour
    {
        [Space(10f)]
        [Tooltip("Add any EffectTargets here that will be added to the actor once this weapon is equipped.")]
        [SerializeField] protected EffectTarget[] effectTargets;
        
        public UnityEvent OnUseStart;
        public UnityEvent OnUseEnd;
        public ActorWeapon WeaponData { get; set; }

        public EffectTarget[] EffectTargets => effectTargets;
        
        [ContextMenu("Update Effect Targets")]
        public void UpdateEffectTargets()
        {
            effectTargets = GetComponentsInChildren<EffectTarget>(true);
            #if UNITY_EDITOR
            // Set this prefab as Dirty in the editor
            UnityEditor.EditorUtility.SetDirty(gameObject);
            #endif
        }
    }
}
