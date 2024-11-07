using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Utilities
{
    public class TimedEvent : MonoBehaviour
    {
        [SerializeField] private bool _runOnEnable;
        
        [SerializeField] private float time;

        public UnityEvent OnTimedEvent;
        
        private void OnEnable()
        {
            if (_runOnEnable)
            {
                Run();
            }
        }
        
        public void Run()
        {
            Invoke(nameof(TimeOut), time);
        }

        private void TimeOut()
        {
            OnTimedEvent?.Invoke();
        }
    }
}
