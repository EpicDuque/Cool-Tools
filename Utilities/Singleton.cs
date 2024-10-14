using UnityEngine;

namespace CoolTools.Utilities
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                // if (_instance == null)
                // {
                //     _instance = FindFirstObjectByType<T>();
                //     if (_instance == null)
                //     {
                //         var o = new GameObject("Singleton_" + typeof(T));
                //         o.AddComponent<T>(); // This should add Instance on Awake
                //     }
                // }

                return _instance;
            }
            
            protected set => _instance = value;
        }

        protected virtual void Awake()
        {
            InitializeSingleton();
        }

        public virtual void InitializeSingleton()
        {
            if (!Application.isPlaying) return;

            if (Instance == null)
            {
                Instance = (T) (object) this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}