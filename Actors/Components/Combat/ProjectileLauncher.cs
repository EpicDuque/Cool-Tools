using System;
using CoolTools.Attributes;
using CoolTools.Utilities;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Actors
{
    public class ProjectileLauncher : OwnableBehaviour
    {
        [Serializable]
        public struct ProjectileLaunchEvents
        {
            public UnityEvent<Projectile> OnLaunched;
        }

        [ColorSpacer("Projectile Launcher")]
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private ObjectPoolConfig _poolConfig;
        
        [Space(10f)] 
        [SerializeField] private LauncherModule _launchModule;
        
        [Space(10f)]
        [SerializeField] private Transform _launchPoint;
        [SerializeField] private FloatValueConfig _launchForce;

        [Space(10f)] 
        [SerializeField] private ProjectileLaunchEvents _events;

        private static ObjectPool Pool;
        private bool _usePool;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void ResetDomain()
        {
            Pool = null;
        }
        
        public IDetectable Target { get; set; }
        public Vector3 TargetPosition { get; set; }
        public bool UseTargetPosition { get; set; }
        public FloatValueConfig LaunchForce => _launchForce;

        public Transform LaunchPoint
        {
            get => _launchPoint;
            set => _launchPoint = value;
        }

        public Projectile ProjectilePrefab
        {
            get => _projectilePrefab;
            set => _projectilePrefab = value;
        }

        public ProjectileLaunchEvents Events => _events;

        private void Start()
        {
            
        }

        protected override void OnStatsUpdated()
        {
            base.OnStatsUpdated();
            
            _launchForce.UpdateValue(Owner.Evaluator, new EvaluateParams
            {
                Source = Owner,
            });
        }

        private void OnValidate()
        {
            _launchForce.UpdateValue(this);
        }

        protected new void Awake()
        {
            base.Awake();

            _usePool = _poolConfig != null;
        }

        [UsedImplicitly]
        public virtual void Launch()
        {
            if (_usePool)
            {
                if (Pool == null)
                    Pool = ObjectPool.GetPool(_poolConfig.PoolName);

                if (Pool == null) return;
                
            }
            
            _launchModule.SetPool(Pool);
            
            var p = _launchModule.Launch(this);
            
            _events.OnLaunched?.Invoke(p);
        }
    }
}