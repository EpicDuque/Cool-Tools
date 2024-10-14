using UnityEngine;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New Animation Event", menuName = "Actor/Animation Event", order = 0)]
    public class AnimationEventSO : ScriptableObject
    {
        [SerializeField] private string _eventName;
        [SerializeField] private bool _endsAbility;

        public string EventName => _eventName;
        public bool EndsAbility => _endsAbility;
    }
}