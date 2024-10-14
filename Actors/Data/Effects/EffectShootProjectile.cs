using UnityEngine;
using UnityEngine.Serialization;

namespace CoolTools.Actors
{
    [CreateAssetMenu(fileName = "New Shoot Projectile Effect", menuName = "Effect/Shoot Projectile")]
    public class EffectShootProjectile : EffectBase
    {
        [Space(10f)]
        [SerializeField] private ProjectileLauncher launcherPrefab;
        [SerializeField] private EffectTargetTag launcherTag;
        
        [Space(10f)]
        [SerializeField] private Projectile projectilePrefab;
        
        [Space(10f)]
        [SerializeField] private int burst = -1;
        [SerializeField] private float burstInterval = -1f;
        
        public override void ExecuteEffect(Actor target, Actor source = null)
        {
            base.ExecuteEffect(target, source);
            
            var obj = target.FindEffectTarget(launcherTag);

            if (obj == null) return;

            ProjectileLauncher launcher;

            if (launcherPrefab != null)
            {
                launcher = Instantiate(launcherPrefab, obj.transform.position, obj.transform.rotation);
                launcher.Owner = target;
            }
            else if (!obj.TryGetComponent(out launcher)) return;

            if(projectilePrefab != null)
                launcher.ProjectilePrefab = projectilePrefab;

            if (burst > 0)
                launcher.BurstAmount = burst;
            
            if(burstInterval > 0)
                launcher.BurstPeriod = burstInterval;

            launcher.LaunchProjectiles();
        }
        
    }
}