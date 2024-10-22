using System;
using System.Collections.Generic;
using CoolTools.Attributes;
using CoolTools.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace CoolTools.Actors
{
    public class OwnableBehaviour : MonoBehaviour, IOwnable
    {
        [ColorSpacer("Ownable", 0.7f, 0.7f, 0.85f, 1f)]
        [SerializeField] private Actor _owner;
        // [Tooltip("When obtaining an OwnableBehaviour from this object via GetComponent or similar, " +
        //          "gives this instance priority over the other OwnableBehaviours.")]
        // [SerializeField] protected bool _root;
        [Tooltip("Automatically gets ownership from this Ownable.")]
        [SerializeField] private OwnableBehaviour getOwnershipFrom;
        
        private IDisposable statProviderSubscribeDisposable;

        public Actor Owner
        {
            get => _owner;
            set
            {
                PreviousOwner = _owner;
                _owner = value;

                OnOwnerChanged(value);
            }
        }

        public Actor PreviousOwner { get; private set; }

        public OwnableBehaviour GetOwnershipFrom
        {
            get => getOwnershipFrom;
            set => getOwnershipFrom = value;
        }

        public event Action<Actor> OwnershipChanged;
        public event Action Disposed;

        /// <summary>
        /// This can be used instead of a null check for Ownership
        /// </summary>
        public bool HasOwner { get; private set; }

        protected void Reset()
        {
            Owner = GetComponentInParent<Actor>();
        }
        
        protected void Awake()
        {
            if(Owner != null)
                OnOwnerChanged(Owner);

            if (GetOwnershipFrom != null)
            {
                GetOwnershipFrom.OwnershipChanged += OnGetOwnershipOwnerChanged;
                
                Observable.EveryUpdate().Take(2).Subscribe(_ =>
                {
                    OnGetOwnershipOwnerChanged(GetOwnershipFrom.Owner);
                }).AddTo(this);
            }
        }
        
        protected virtual void OnDestroy()
        {
            if (GetOwnershipFrom != null)
                GetOwnershipFrom.OwnershipChanged -= OnGetOwnershipOwnerChanged;

            if (HasOwner)
            {
                Owner.UnregisterOwnership(this);
                
                if(Owner.HasStatProvider)
                    Owner.StatProvider.StatsUpdated -= OnStatsUpdated;
            }
        }

        protected virtual void OnGetOwnershipOwnerChanged(Actor newOwner)
        {
            Owner = newOwner;
        }

        /// <summary>
        /// Called right after assigning Ownership to this OwnableBehaviour
        /// </summary>
        /// <param name="newOwner">The new owner</param>
        protected virtual void OnOwnerChanged(Actor newOwner)
        {
            HasOwner = newOwner != null;
            if (!Application.isPlaying) return;
            
            if (PreviousOwner != null)
            {
                PreviousOwner.UnregisterOwnership(this);
                
                if (PreviousOwner.HasStatProvider)
                {
                    PreviousOwner.StatProvider.StatsUpdated -= OnStatsUpdated;
                }
            }

            if (!HasOwner) return;
            OwnershipChanged?.Invoke(newOwner);
            
            Owner.RegisterOwnership(this);
            
            if (Owner.HasStatProvider)
            {
                Owner.StatProvider.StatsUpdated += OnStatsUpdated;
                OnStatsUpdated();
            }
            else
            {
                statProviderSubscribeDisposable?.Dispose();
                statProviderSubscribeDisposable = Observable.EveryUpdate()
                    .First(_ => Owner.HasStatProvider).Subscribe(_ =>
                {
                    Owner.StatProvider.StatsUpdated += OnStatsUpdated;
                    OnStatsUpdated();
                }).AddTo(this);
            }
        }

        /// <summary>
        /// Called when the Owner's StatProvider updates its stats
        /// </summary>
        protected virtual void OnStatsUpdated()
        {
        }

        /// <summary>
        /// Spawns an IOwnable GameObject and assigns this object's Owner as the spawned object's Owner.
        /// </summary>
        /// <returns>The instanced object with assigned ownership.</returns>
        public T SpawnWithOwnership<T>(T obj, Transform parent) where T : MonoBehaviour, IOwnable
        {
            var b = SpawnWithOwnership(obj, parent.position, parent.rotation);
            var t= b.transform;
            
            t.SetParent(parent);
            t.localPosition = Vector3.zero;

            return b;
        }

        /// <summary>
        /// Spawns an IOwnable GameObject and assigns this object's Owner as the spawned object's owner.
        /// </summary>
        /// <returns>The instanced object with assigned ownership.</returns>
        public T SpawnWithOwnership<T>(T obj, Vector3 position, Quaternion rotation) where T : MonoBehaviour, IOwnable
        {
            var b = Instantiate(obj, position, rotation);
            b.Owner = Owner;

            return b;
        }
        
        public T SpawnWithOwnership<T>(T obj, Vector3 position, Quaternion rotation, ObjectPool pool) where T : MonoBehaviour, IOwnable
        {
            var b = pool.Pull<T>(obj.name, position, rotation, gameObject.scene);
            b.Owner = Owner;

            return b;
        }
        
        public virtual void Dispose(float delay = 0f, bool doNotDestroy = false)
        {
            Disposed?.Invoke();
            
            if(!doNotDestroy)
                Destroy(gameObject, delay);
        }
    }
}