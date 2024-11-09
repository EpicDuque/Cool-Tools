using System;
using System.Collections.Generic;
using System.Linq;
using CoolTools.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Actors
{
    /// <summary>
    /// Main component of Actor Engine. Actor contains basic functionality that the rest of the components of the Engine
    /// refer to.
    /// </summary>
    public class Actor : MonoBehaviour
    {
        [Serializable]
        public class ActorEvents
        { 
            public UnityEvent<Actor> OnActorInitialized;
            public UnityEvent<Actor> OnActorEnabled;
            public UnityEvent<Actor> OnActorDisabled;
            public UnityEvent<Actor> OnActorDestroyed;
        }

        #region Serialized Fields

        [Serializable]
        public enum ActorInitMode
        {
            Awake, Enable, Start, Script
        }
        
        [ColorSpacer("Actor")]
        [SerializeField] private ActorInitMode initMode = ActorInitMode.Start;
        [SerializeField] private bool _autoDetectComponents = false;

        [Header("References")] 
        [SerializeField] private GameObject _model;
        [SerializeField] protected StatProvider _statProvider;
        [SerializeField] protected DamageableResource _damageableResource;
        [SerializeField] protected Animator _animator;
        
        [Space(10f)]
        [SerializeField, InspectorDisabled] private List<EffectTarget> _effectTargets = new();
        
        [Header("Data")]
        [SerializeField] private string actorName = "Actor";
        [SerializeField] protected ActorFaction _faction;
        [SerializeField] protected ActorFormulaEvaluator _evaluator;

        [Header("Events")]
        [SerializeField] protected ActorEvents _actorEvents;
        
        #endregion

        #region Private Fields

        private List<OwnableBehaviour> ownedBehaviours = new();

        #endregion
        
        #region Public Properties

        public string ActorName
        {
            get => actorName;
            set => actorName = value;
        }

        public OwnableBehaviour[] OwnedBehaviours => ownedBehaviours.ToArray();

        public StatProvider StatProvider
        {
            get => _statProvider;
            set => _statProvider = value;
        }
        
        public bool HasDamageable { get; protected set; }
        public bool HasStatProvider { get; protected set; }
        public bool HasAnimator { get; protected set; }

        public DamageableResource DamageableResource
        {
            get => _damageableResource;
            set => _damageableResource = value;
        }

        public ActorFaction Faction
        {
            get => _faction;
            set => _faction = value;
        }

        public ActorFormulaEvaluator Evaluator => _evaluator;

        public Animator Animator => _animator;

        public ActorEvents Events => _actorEvents;

        public ActorInitMode InitMode
        {
            get => initMode;
            set => initMode = value;
        }

        public bool IsInitialized { get; set; }

        public GameObject Model => _model;

        #endregion

        #region MonoBehaviour Events

        private void OnValidate()
        {
            if (!_autoDetectComponents) return;
            
            SearchForBasicComponents();
        }

        private new void Awake()
        {
            if(initMode == ActorInitMode.Awake)
                Initialize();
        }

        protected void OnEnable()
        {
            if(initMode == ActorInitMode.Enable)
                Initialize();
            
            Events.OnActorEnabled?.Invoke(this);
        }

        protected void Start()
        {
            if(initMode == ActorInitMode.Start)
                Initialize();
        }
        
        private void OnDisable()
        {
            Events.OnActorDisabled?.Invoke(this);
        }

        protected new void OnDestroy()
        {
            Events.OnActorDestroyed?.Invoke(this);
        }

        #endregion

        public void SearchForBasicComponents()
        {
            StatProvider = GetComponentInChildren<StatProvider>();
            DamageableResource = GetComponentInChildren<DamageableResource>();
            _animator = GetComponentInChildren<Animator>();
            
            HasDamageable = DamageableResource != null;
            HasStatProvider = StatProvider != null;
            HasAnimator = Animator != null;
        }
        
        public virtual void Initialize()
        {
            if (IsInitialized) return;
            
            HasDamageable = DamageableResource != null;
            HasStatProvider = StatProvider != null;
            HasAnimator = Animator != null;
            
            if (HasStatProvider)
            {
                StatProvider.Initialize();
            }
            
            UpdateEffectTargets();

            Events.OnActorInitialized?.Invoke(this);
            IsInitialized = true;
        }

        /// <summary>
        /// Updates the Actor's EffectTargets list from EffectTargets in the Actors hierarchy.
        /// </summary>
        public void UpdateEffectTargets()
        {
            _effectTargets.Clear();
            
            _effectTargets.AddRange(GetComponentsInChildren<EffectTarget>(true)
                .Where(o => o != null));

            // foreach (var disposable in effectDestroyDisposables)
            //     disposable.Dispose();
            //
            // effectDestroyDisposables.Clear();

            // if (!Application.isPlaying) return;
            
            // foreach (var target in _effectTargets)
            // {
            //     if (target.gameObject.TryGetComponent<ObservableDestroyTrigger>(out var component))
            //     {
            //         Destroy(component);
            //     }
            //     effectDestroyDisposables.Add(target.gameObject.OnDestroyAsObservable().First().Subscribe(_ =>
            //     {
            //         _effectTargets.Remove(target);
            //     }).AddTo(this));
            // }
        }
        
        public void AddEffectTargets(IEnumerable<EffectTarget> targets)
        {
            _effectTargets.AddRange(targets);
        }
        
        /// <summary>
        /// Finds an EffectTarget with the given tag in the Actor's EffectTargets list.
        /// </summary>
        /// <param name="targetTag">Tag Asset to identify target.</param>
        /// <returns>Matching EffectTarget component in Actor's hierarchy.</returns>
        public EffectTarget FindEffectTarget(EffectTargetTag targetTag)
        {
            EffectTarget target = null;
            foreach (var o in _effectTargets)
            {
                if (o != null && o.TargetTag == targetTag)
                {
                    target = o;
                    break;
                }
            }

            if (target != null)
                return target;
                
            Debug.LogError($"[{nameof(Actor)}] Effect Target {targetTag.TagName} not found in Actor: {name}");
            
            return null;
        }

        public bool TryGetEffectTarget(EffectTargetTag effectTag, out EffectTarget target)
        {
            var obj = FindEffectTarget(effectTag);
            target = obj;

            return obj != null;
        }

        public IEnumerable<EffectTarget> FindAllEffectTargets(IEnumerable<EffectTargetTag> tags)
        {
            var tagsArray = tags.ToList();
            
            return _effectTargets.Where(et => et != null && tagsArray.Contains(et.TargetTag));
        }

        protected internal void RegisterOwnership(OwnableBehaviour ownable) => ownedBehaviours.Add(ownable);
        
        protected internal void UnregisterOwnership(OwnableBehaviour ownable) => ownedBehaviours.Remove(ownable);

        public T GetOwnableBehaviour<T>() where T : OwnableBehaviour
        {
            return OwnedBehaviours.FirstOrDefault(ob => ob is T) as T;
        }
        
        public T[] GetOwnableBehaviours<T>() where T : OwnableBehaviour
        {
            return OwnedBehaviours.Where(ob => ob is T).Cast<T>().ToArray();
        }
    }
}
