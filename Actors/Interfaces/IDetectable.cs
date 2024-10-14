using UnityEngine;

namespace CoolTools.Actors
{
    public interface IDetectable
    {
        public GameObject GO { get; }
        public Transform TargetPoint { get; set; }
        public bool BypassDetection { get; set; }
    }
}