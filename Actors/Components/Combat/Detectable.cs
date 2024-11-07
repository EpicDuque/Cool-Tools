using CoolTools.Actors;
using CoolTools.Attributes;
using UnityEngine;

namespace CoolTools.Actors
{
    public class Detectable : OwnableBehaviour, IDetectable
    {
        [ColorSpacer("Detectable")]
        [SerializeField] private Transform _targetPoint;
        [SerializeField] private bool _bypassDetection;
        [SerializeField] private bool _trackingPositionGizmo;

        private MovementBehaviour _movement;
        private Rigidbody _rb;
        
        public GameObject GO => gameObject;
        
        public Transform TargetPoint
        {
            get => _targetPoint;
            set => _targetPoint = value;
        }
        
        public bool BypassDetection
        {
            get => _bypassDetection;
            set => _bypassDetection = value;
        }
        
        public static Detectable Null;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            Null = null;
            var obj = new GameObject("NULL");
            obj.hideFlags |= HideFlags.None;
            Null = obj.AddComponent<Detectable>();
            
            DontDestroyOnLoad(Null.gameObject);
        }

        protected new void Awake()
        {
            base.Awake();
            
            _movement = GetComponent<MovementBehaviour>();
            _rb = GetComponent<Rigidbody>();
        }
    }
}