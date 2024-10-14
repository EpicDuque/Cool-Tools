using UnityEngine;

namespace CoolTools.Data
{
    [CreateAssetMenu(fileName = "New Void Event", menuName = "Event/Void")]
    public class VoidEvent : GameEventSO<Void>
    {
        public void Raise() => base.Raise(new Void());
    }
}