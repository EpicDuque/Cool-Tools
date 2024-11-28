using UnityEngine;

namespace CoolTools.Utilities
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get => _instance;

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