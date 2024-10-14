using CoolTools.Utilities;
using UnityEngine;

namespace CoolTools.Actors
{
    public class SpawnOnProjectileDeath : MonoBehaviour
    {
        [SerializeField] private ObjectPoolConfig _poolConfig;
        [SerializeField] private GameObject _prefabSpawn;
        [SerializeField] private bool _spawnWithOwnership;
        
        private Projectile _projectile;
        private ObjectPool _pool;

        private void Awake()
        {
            _projectile = GetComponent<Projectile>();
            _pool = ObjectPool.GetPool(_poolConfig.PoolName);
            
            _projectile.Events.OnDestroyed.AddListener(OnDestroyed);
        }

        private void OnDestroyed()
        {
            if (_prefabSpawn == null) return;
            
            var spawned = _pool.Pull(_prefabSpawn.name, transform.position, transform.rotation, gameObject.scene);

            var obj = spawned == null ? 
                Instantiate(_prefabSpawn, transform.position, transform.rotation) : 
                spawned.gameObject;
            
            if (_spawnWithOwnership && obj.TryGetComponent(out RootOwnable ownable))
            {
                ownable.Owner = _projectile.Owner;
            }
        }
    }
}