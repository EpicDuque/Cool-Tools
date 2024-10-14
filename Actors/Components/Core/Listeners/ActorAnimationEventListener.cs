using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace CoolTools.Actors
{
    public class ActorAnimationEventListener : MonoBehaviour
    {
        public UnityEvent<AnimationEventSO> OnEvent;
        
        public AnimationEventSO LastEvent { get; private set; }

        private void Event(Object ev)
        {
            if (ev is not AnimationEventSO animEvent)
            {
                Debug.LogWarning($"[{nameof(ActorAnimationEventListener)}] Animation event Object input must be of type {nameof(AnimationEventSO)}");
                return;
            }

            OnEvent?.Invoke(animEvent);
        }
    }
}