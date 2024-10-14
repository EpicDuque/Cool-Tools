﻿using CoolTools.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace CoolTools.Actors
{
    public class BuffData : ScriptableObject
    {
        [SerializeField] private string _buffName;
        [SerializeField, TextArea] private string _description;
        
        [Space(10)] 
        [SerializeField] private EffectBase[] _effects;
        
        [Space(10f)] 
        [SerializeField] private GameObject _visualPrefab;
        [FormerlySerializedAs("_pool")] 
        [SerializeField] private ObjectPoolConfig _poolConfig;
        
        public EffectBase[] Effects => _effects;

        public GameObject GetVisual()
        {
            if (_poolConfig != null)
            {
                var pool = ObjectPool.GetPool(_poolConfig);
                var activeScene = SceneManager.GetActiveScene();
                var instance = pool.Pull(_visualPrefab.name, Vector3.zero, Quaternion.identity, activeScene);
                instance.gameObject.SetActive(true);

                return instance.gameObject;
            }
            else
            {
                var instance = Instantiate(_visualPrefab);
                return instance;
            }
        }
    }
}