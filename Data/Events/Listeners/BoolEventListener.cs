using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Data
{
    public class BoolEventListener : GameEventListener<BoolEvent, bool>
    {
        [Space(10f)] 
        public UnityEvent OnTrue;
        public UnityEvent OnFalse;

        public override void OnEventRaised(bool data)
        {
            base.OnEventRaised(data);
            
            if (data)
                OnTrue?.Invoke();
            else
                OnFalse?.Invoke();
        }
    }
}