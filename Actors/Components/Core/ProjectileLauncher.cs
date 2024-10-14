using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace CoolTools.Actors
{
    public class ProjectileLauncher : OwnableBehaviour
    {
        [Space(10f)]
        [SerializeField] protected Projectile projectilePrefab;

        [Space(10f)] 
        [SerializeField] protected Transform launchPos;
        
        [Space(10f)] 
        [SerializeField] private ActorFormulaEvaluator evaluator;
        
        [Space(5f)]
        [SerializeField] private Formula formulaBurstAmount;
        [SerializeField] protected int burstAmount;
        [SerializeField] protected float burstPeriod = 0.1f;
        
        [Space(5f)]
        [SerializeField] private Formula formulaLaunchImpulse;
        [SerializeField] protected float launchImpulse;

        [Space(10f)] 
        [SerializeField] protected List<ProjectileLauncher> subLaunchers;

        [Space(10f)]
        public UnityEvent<Projectile> OnPreLaunch;
        public UnityEvent<Projectile> OnLaunch;
        public UnityEvent<Projectile> OnPostLaunch;
        
        private Coroutine routine;
        private bool launching;

        public Projectile ProjectilePrefab
        {
            get => projectilePrefab;
            set => projectilePrefab = value;
        }

        public int BurstAmount
        {
            get => burstAmount;
            set => burstAmount = value;
        }

        public float BurstPeriod
        {
            get => burstPeriod;
            set => burstPeriod = value;
        }

        protected void OnValidate()
        {
            if (Owner == null) return;
            
            OnStatsUpdated();
        }

        protected override void OnOwnerChanged(Actor newOwner)
        {
            base.OnOwnerChanged(newOwner);
            
            OnStatsUpdated();
        }

        protected override void OnStatsUpdated()
        {
            if (!HasOwner) return;
            
            burstAmount = Mathf.RoundToInt(evaluator.Evaluate(formulaBurstAmount, Owner, burstAmount));
            launchImpulse = evaluator.Evaluate(formulaLaunchImpulse, Owner, launchImpulse);
        }

        public virtual void LaunchProjectiles()
        {
            if (launching) return;
            
            launching = true;
            OnPreLaunch?.Invoke(projectilePrefab);

            if (BurstAmount > 1)
            {
                var observable = Observable.Interval(TimeSpan.FromSeconds(BurstPeriod))
                    .Take(BurstAmount-1);
                    
                observable.Subscribe(_ => LaunchProjectile(projectilePrefab)).AddTo(this);
                LaunchProjectile(projectilePrefab);

                observable.Last().Subscribe(_ =>
                {
                    OnPostLaunch?.Invoke(projectilePrefab);
                        
                    foreach(var launcher in subLaunchers)
                        launcher.LaunchProjectiles();

                    launching = false;

                }).AddTo(this);
            }
            else
            {
                LaunchProjectile(projectilePrefab);
                OnPostLaunch?.Invoke(projectilePrefab);
                        
                foreach(var launcher in subLaunchers)
                    launcher.LaunchProjectiles();

                launching = false;
            }
        }

        protected virtual Projectile LaunchProjectile(Projectile p)
        {
            var pr = SpawnWithOwnership(p, launchPos.position, launchPos.rotation);
            pr.Init(this);
            
            ImpulseProjectile(pr);
            
            OnLaunch?.Invoke(pr);
            
            return pr;
        }

        protected virtual void ImpulseProjectile(Projectile pr)
        {
            pr.RigidBody.velocity = (transform.forward * launchImpulse);
        }
    }
}