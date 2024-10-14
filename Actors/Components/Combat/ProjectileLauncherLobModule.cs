using UniRx;
using UnityEngine;

namespace CoolTools.Actors
{
    [RequireComponent(typeof(ProjectileLauncher))]
    public class ProjectileLauncherLobModule : MonoBehaviour
    {
        [SerializeField] private ProjectileLauncher _launcher;
        
        [Space(10f)]
        [SerializeField] private float _lobPeriodFactor = 0.15f;
        [SerializeField] private bool _orientTransform;

        private void OnValidate()
        {
            // if(_launcher != null)
            //     _launcher.OverrideLaunch = true;
        }

        private void Awake()
        {
            _launcher.Events.OnLaunched.AsObservable()
                .Subscribe(OnLaunched).AddTo(this);

            // _launcher.OverrideLaunch = true;
        }

        private void OnLaunched(Projectile projectile)
        {
            var period = Vector3.Distance(projectile.transform.position, _launcher.TargetPosition) *  _lobPeriodFactor;

            projectile.RB.velocity = Vector3.zero;
            projectile.HomingFactor = 0f;
            projectile.RB.isKinematic = false;
            projectile.RB.useGravity = true;
            projectile.RB.drag = 0f;
            projectile.RB.angularDrag = 0f;

            Vector3 velocity;
            
            if (_launcher.UseTargetPosition)
            {
                velocity = DetermineLobInitVel(projectile.transform.position, _launcher.TargetPosition, period);

                
            } else if (_launcher.Target != null)
            {
                velocity = DetermineLobInitVel(projectile.transform.position,
                    _launcher.Target.TargetPoint.position, period);
            }
            else return;
            
            if (_orientTransform)
            {
                transform.rotation = Quaternion.LookRotation(velocity);
            }
            projectile.RB.velocity = velocity;
        }
        
        public Vector3 DetermineLobInitVel(Vector3 pos, Vector3 target, float period)
        {
            var myPos = pos;

            var velZ = (target.z - myPos.z) / period;
            var velX = (target.x - myPos.x) / period;
            var velY = (target.y - myPos.y - 0.5f * Physics.gravity.y * Mathf.Pow(period, 2)) / period;

            return new Vector3(velX, velY, velZ);
        }
    }
}