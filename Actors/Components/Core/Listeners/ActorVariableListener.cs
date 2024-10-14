using CoolTools.Data;
using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Actors
{
    public class ActorVariableListener : VariableEventListener<ActorVariable, Actor>
    {
        public UnityEvent<Transform> ResposeAsTransform;
        public UnityEvent<GameObject> ResposeAsGameObject;

        protected override void OnValueChanged(Actor actor)
        {
            ResposeAsTransform?.Invoke(actor.transform);
            ResposeAsGameObject?.Invoke(actor.gameObject);
        }
    }
}