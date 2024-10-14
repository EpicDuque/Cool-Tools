﻿using CoolTools.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Utilities
{
    public class PoolableObject : MonoBehaviour
    {
        [field:SerializeField, InspectorDisabled] public ObjectPool Pool { get; set; }

        public Transform PoolParent;
        public UnityEvent OnObjectPulled;

        public virtual void Initialize()
        {
            gameObject.SetActive(true);
            OnObjectPulled?.Invoke();
        }
        
        public virtual void ReturnToPool()
        {
            if (Pool == null)
            {
                Destroy(gameObject);
                return;
            }
            
            gameObject.SetActive(false);
            Pool.Put(this);
        }

        public static void DestroyOrReturn(GameObject obj)
        {
            if (obj.TryGetComponent<PoolableObject>(out var poolable))
            {
                poolable.ReturnToPool();
            }
            else
            {
                Destroy(obj);
            }
        }
    }
}