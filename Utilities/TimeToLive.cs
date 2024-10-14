using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Utilities
{
    public class TimeToLive : MonoBehaviour
    {
        [SerializeField] private float _timeToLive;
        [SerializeField] private bool _autoDispose;

        public UnityEvent OnTimeOut;
        
        private PoolableObject _poolableObject;
        private bool _hasPoolableObject;
        private float _count;

        private void Awake()
        {
            _hasPoolableObject = TryGetComponent(out _poolableObject);
        }

        private void OnEnable()
        {
            _count = 0f;
        }

        private void OnDisable()
        {
            _count = 0f;
        }

        private void Update()
        {
            _count += Time.deltaTime;

            if (_count >= _timeToLive)
            {
                OnTimeOut?.Invoke();
                DestroyObject();
            }
        }

        private void DestroyObject()
        {
            if (!_autoDispose) return;
            
            if (_hasPoolableObject)
            {
                _poolableObject.ReturnToPool();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}