using UnityEngine;

namespace CoolTools.Actors
{
    public interface ITargetPositionAquireable
    {
        public Vector3 TargetPosition { get; set; }
    }
    
    public interface ITargetDamageableAquireable
    {
        public IDamageable Target { get; set; }
    }
    
    public interface ITargettableAquireable
    {
        public ITargettable Target { get; set; }
    }
}