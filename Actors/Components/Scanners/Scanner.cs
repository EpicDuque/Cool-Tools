using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using CoolTools.Attributes;
using CoolTools.Utilities;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;

namespace CoolTools.Actors
{
    public class Scanner<T> : OwnableBehaviour where T : Component
    {
        [SerializeField] protected int maxCount = 15;
        [SerializeField] protected float updatePeriod = 0.1f;
        [SerializeField] protected bool includeInactive;
        [SerializeField] protected bool sortByDistance;

        [ColorSpacer("Line of Sight")]
        [SerializeField] protected Transform lineOfSightOrigin;
        [SerializeField] protected LayerMask lineOfSightLayers;
        
        [Space(10f)]
        [SerializeField, InspectorDisabled] protected List<T> nearObjects = new();
        
        protected Dictionary<T, Collider> objectColliders = new();
        private Coroutine updateCoroutine;
        private List<T> invalidObjects = new();
        // private List<T> orderedObjects = new();
        private DistanceComparator distanceComparator = new();
        private Collider[] attachedColliders;
        
        public int MaxCount
        {
            get => maxCount;
            set => maxCount = value;
        }

        public T[] NearObjects => nearObjects.ToArray();
        
        public T FirstItem => nearObjects.FirstOrDefault();

        private new void Awake()
        {
            base.Awake();
            
            attachedColliders = GetComponents<Collider>();
        }

        private void OnEnable()
        {
            updateCoroutine = StartCoroutine( CheckRoutine() );
        }

        private void OnDisable()
        {
            StopCoroutine(updateCoroutine);
        }

        private IEnumerator CheckRoutine()
        {
            var delay = new WaitForSeconds(updatePeriod);

            while (true)
            {
                UpdateObjects();
                yield return delay;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (objectColliders.ContainsValue(other)) return;
            if (!other.TryGetComponent<T>(out var o)) return;
            if (nearObjects.Contains(o)) return;
            
            if (SelfCheck(o)) return;
            if (!IsTargetValid(o)) return;
            
            objectColliders.Add(o, other);
            
            if (IsPositionInLineOfSight(other.transform.position))
                nearObjects.Add(o);

            var destroyComponent = o.OnDestroyAsObservable().First();
            var destroyObject = o.gameObject.OnDestroyAsObservable().First();
            
            Observable.Merge(new []{destroyComponent, destroyObject})
                .First()
                .Subscribe(_ =>
                {
                    objectColliders.Remove(o);
                    
                    if(nearObjects.Contains(o))
                        nearObjects.Remove(o);
                }).AddTo(this);
            
            OnObjectAdded(o);
        }

        protected virtual void OnObjectAdded(T obj)
        {
        }

        protected void OnTriggerExit(Collider other)
        {
            if (!objectColliders.ContainsValue(other)) return;
            
            var remove = objectColliders.FirstOrDefault(x => x.Value == other).Key;
            nearObjects.Remove(remove);
            objectColliders.Remove(remove);
        }

        protected virtual bool IsTargetValid(T component)
        {
            return component != null;
        }

        /// <summary>
        /// Checks if the detected object is this.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected virtual bool SelfCheck(T other) => other.gameObject == gameObject;

        private bool IsPositionInLineOfSight(Vector3 pos)
        {
            if (lineOfSightLayers.value == 0) return true;
            
            var origin = lineOfSightOrigin.position + Vector3.up;
            var target = pos + Vector3.up;
            
            return !origin.IsLineObstructed(target, lineOfSightLayers, gameObject.scene);
        }
        
        private void UpdateObjects()
        {
            invalidObjects.Clear();
            
            foreach(var (key, value) in objectColliders)
            {
                if (key == null || value == null)
                {
                    invalidObjects.Add(key);
                    continue;
                }
        
                var contains = false;
                if (IsPositionInLineOfSight(key.transform.position))
                {
                    foreach (var no in nearObjects)
                    {
                        if (no != key) continue;
                        
                        contains = true;
                        break;
                    }
                    
                    if(!contains)
                        nearObjects.Add(key);
                }
                else
                {
                    contains = false;
                    foreach (var no in nearObjects)
                    {
                        if (no != key) continue;
                        contains = true;
                        break;
                    }
                    
                    if(contains)
                        nearObjects.Remove(key);
                }
            }
            
            foreach (var o in invalidObjects)
            {
                objectColliders.Remove(o);
                
                if(nearObjects.Contains(o))
                    nearObjects.Remove(o);
            }
        
            if (sortByDistance)
            {
                distanceComparator.Source = transform.position;
        
                nearObjects.Sort(distanceComparator);
            }
        }

        private class DistanceComparator : IComparer<T>
        {
            public Vector3 Source;
            
            public int Compare(T x, T y)
            {
                return (x.transform.position - Source).sqrMagnitude.CompareTo((y.transform.position - Source).sqrMagnitude);
            }
        }
    }
}