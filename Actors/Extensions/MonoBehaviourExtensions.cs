using CoolTools.Utilities;
using UnityEngine;

namespace CoolTools.Actors
{
    public static class MonoBehaviourExtensions
    {
        public static void DestroyOrReturn(this MonoBehaviour mono)
        {
            PoolableObject.DestroyOrReturn(mono.gameObject);
        }
    }
}