using System;
using UnityEngine;

namespace CoolTools.Actors
{
    public interface IEffectWaiter
    {
        public float Duration { get; }

        public WaitForSeconds Wait { get; }
    }
}