using UnityEngine;
using CoolTools.Utilities;
using UnityEngine.SceneManagement;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New Launcher Module", menuName = "Modules/Launcher/Linear (Default)", order = 0)]
    public class LauncherModule : ScriptableObject
    {
        [SerializeField] private bool _zeroY = true;
        [SerializeField] private ForceMode _forceMode = ForceMode.VelocityChange;

        private bool _usePool;
        private ObjectPool _pool;
        
        public virtual void SetPool(ObjectPool pool)
        {
            _pool = pool;
            _usePool = _pool != null;
        }
        
        public virtual Projectile Launch(ProjectileLauncher launcher)
        {
            var launchPoint = launcher.LaunchPoint;
            var scene = launcher.gameObject.scene;
            
            var projectile = CreateProjectile(launcher, launchPoint, scene);

            var launchForce = launcher.LaunchForce.Value;
            
            InitializeProjectile(launcher, projectile);
            
            if (_zeroY)
            {
                projectile.transform.forward = new Vector3(launchPoint.forward.x, 0f, launchPoint.forward.z);
            }
            else
            {
                projectile.transform.forward = launchPoint.forward.normalized;
            }

            var shootDirection = projectile.transform.forward;

            var rb = projectile.RB;
            
            rb.isKinematic = false;
            rb.ResetInertiaTensor();
            rb.ResetCenterOfMass();
            
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
            rb.AddForce(shootDirection * launchForce, _forceMode);
            
            return projectile;
        }

        protected void InitializeProjectile(ProjectileLauncher launcher, Projectile projectile)
        {
            if(launcher.Target != null)
                projectile.Initialize(launcher.Target);
            else if(launcher.UseTargetPosition)
                projectile.Initialize(launcher.TargetPosition);
            else
                projectile.Initialize();

            projectile.OriginLauncher = launcher;
        }

        protected virtual Projectile CreateProjectile(ProjectileLauncher launcher, Transform launchPoint, Scene scene)
        {
            Projectile projectile = null;
            if (_usePool)
            {
                projectile = _pool.Pull<Projectile>(launcher.ProjectilePrefab.name, 
                    launchPoint.position, launchPoint.rotation, scene);
            }
            
            if (!_usePool || projectile == null)
            {
                projectile = Instantiate(launcher.ProjectilePrefab, launchPoint.position, launchPoint.rotation);
            }
            
            projectile.Owner = launcher.Owner;

            return projectile;
        }
    }
}