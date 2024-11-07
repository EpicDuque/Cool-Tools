using UnityEngine;

namespace CoolTools.Actors
{
    public interface ITargettable
    {
        public Transform TargetPoint { get; set; }
        public bool BypassDetection { get; set; }
    }
}