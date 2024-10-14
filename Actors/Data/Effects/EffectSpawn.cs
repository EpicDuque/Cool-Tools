using CoolTools.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New Effect Spawn", menuName = "Actor/Effects/Spawn", order = 0)]
    public class EffectSpawn : EffectBase
    {
        [SerializeField] private GameObject _spawnObject;
        
        [Space(10f)]
        [SerializeField] private bool _usePool;
        [SerializeField] private ObjectPoolConfig _poolConfig;
        
        [Space(10f)]
        [SerializeField] private bool _spawnWithOwnership;
        
        [Space(10f)] 
        [SerializeField] private bool _localOffset;
        [SerializeField] private Vector3 _offset;
        
        public override void Execute(Actor source)
        {
            var position = _localOffset ? 
                source.transform.position + source.transform.TransformDirection(_offset) :
                source.transform.position + _offset;
            
            var spawn = SpawnOrPull(_spawnObject, position, source.transform.rotation);
            
            if(_spawnWithOwnership)
                AssignOwnership(source, spawn);
        }

        public override void Execute(Actor source, IDetectable target)
        {
            var position = _localOffset ? 
                target.TargetPoint.position + target.TargetPoint.TransformDirection(_offset) : 
                target.TargetPoint.position + _offset;
            
            // var spawn = Instantiate(_spawnObject, position, target.TargetPoint.rotation);
            var spawn = SpawnOrPull(_spawnObject, position, target.TargetPoint.rotation);
            
            if(_spawnWithOwnership)
                AssignOwnership(source, spawn);
        }
        
        public override void Execute(Actor source, Vector3 target)
        {
            var position = target + _offset;
            
            // var spawn = Instantiate(_spawnObject, position, Quaternion.identity);
            var spawn = SpawnOrPull(_spawnObject, position, Quaternion.identity);
            
            if(_spawnWithOwnership)
                AssignOwnership(source, spawn);
        }

        public override void Execute(Actor source, Actor target)
        {
            Execute(source, target.transform.position);
        }
        
        private GameObject SpawnOrPull(GameObject obj, Vector3 position, Quaternion rotation)
        {
            var activeScene = SceneManager.GetActiveScene();
            
            if (_usePool)
            {
                var pool = ObjectPool.GetPool(_poolConfig.PoolName);
                var instance = pool.Pull(_spawnObject.name, position, rotation, activeScene);

                return instance != null ? instance.gameObject : Instantiate(obj, position, rotation);
            }

            return Instantiate(obj, position, rotation);
        }

        private void AssignOwnership(Actor source, GameObject spawn)
        {
            if (spawn.TryGetComponent(out RootOwnable ownable))
            {
                ownable.Owner = source;
            }
        }
    }
}