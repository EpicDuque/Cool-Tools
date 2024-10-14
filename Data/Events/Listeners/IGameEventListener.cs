using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Data
{
    public interface IGameEventListener<in T>
    {
        public void OnEventRaised(T data);
    }
}