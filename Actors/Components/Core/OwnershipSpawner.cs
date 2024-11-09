using CoolTools.Utilities;
using UnityEngine;

namespace CoolTools.Actors
{
    public class OwnershipSpawner : OwnableBehaviour
    {
        [Space(10f)]
        [SerializeField] private OwnableBehaviour _ownablePrefab;
        [SerializeField] private ObjectPoolConfig _poolConfig;
        
        [Space(10f)]
        [SerializeField] private bool _localOffset;
        [SerializeField] private Vector3 _offset;
        
        [Space(10f)]
        [SerializeField] private bool _inheritRotation = true;
        
        public void Spawn()
        {
            var position = _localOffset ? transform.position + transform.TransformDirection(_offset) : 
                transform.position + _offset;
            
            var rotation = _inheritRotation ? transform.rotation : Quaternion.identity;
            
            SpawnWithOwnership(_ownablePrefab, position, rotation, _poolConfig.Pool);
        }
    }
}