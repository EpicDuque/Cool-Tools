
using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace CoolTools.Actors
{
    public class Projectile : OwnableBehaviour
    {
        [Header("References")]
        [SerializeField] protected HitBox hitBox;
        [SerializeField] protected Rigidbody rb;
        [SerializeField] protected Rigidbody2D rb2d;
        [SerializeField] protected GameObject model;

        [Header("Lifetime")]
        [Tooltip("Negative values mean infinite time to live")]
        [SerializeField] protected float timeToLive = 1f;
        [SerializeField] protected LayerMask destroyAgainstLayers;

        [Header("Trajectory")] 
        [SerializeField] protected float forwardAccel = 0f;
        [SerializeField] protected float maxSpeed = 99f;

        [Header("Raycasting")]
        [HelpBox("Recommended only for fast moving projectiles.")]
        [SerializeField] private bool _raycastDetection;
        [SerializeField] private string _rayCastDetectTag;
        [SerializeField] private LayerMask _rayCastDetectLayers;
        [SerializeField] private int _maxHits = 10;

        [Space(10f)]
        [SerializeField] private OwnableBehaviour spawnOnDestroy;

        private RaycastHit[] _raycastHits;
        
        public UnityEvent Destroyed;
        public Rigidbody RigidBody => rb;
        public Rigidbody2D RigidBody2D => rb2d;

        /// <summary>
        /// Launcher that launched this Projectile
        /// </summary>
        public ProjectileLauncher Launcher { get; set; }

        public HitBox HitBox => hitBox;

        public virtual void Init(ProjectileLauncher launcher)
        {
            Launcher = launcher;
            _raycastHits = new RaycastHit[_maxHits];
            
            if (timeToLive > 0f)
            {
                Observable.Timer(TimeSpan.FromSeconds(timeToLive))
                    .Subscribe(_ => DestroyProjectile()).AddTo(this);
            }

            if (HitBox != null)
            {
                HitBox.OnMaxHitsReached.AddListener(DestroyProjectile);
            }
        }

        public virtual void DestroyProjectile()
        {
            Destroyed?.Invoke();

            if (spawnOnDestroy != null)
            {
                var tr = transform;
                SpawnWithOwnership(spawnOnDestroy, tr.position, Quaternion.identity);
            }
            
            HitBox.OnMaxHitsReached.RemoveListener(DestroyProjectile);

            if (model != null)
                model.SetActive(false);
            Destroy(gameObject, 0.03f);
        }

        protected virtual void UpdateTrajectory()
        {
            RigidBody.AddForce(transform.forward * (forwardAccel * Time.fixedDeltaTime * 100f));
        }

        protected void FixedUpdate()
        {
            if (_raycastDetection)
            {
                // Cast a ray forwards and detect if we hit something in _raycastDetectLayers and _raycastDetectTag
                var hits = Physics.RaycastNonAlloc(transform.position, transform.forward, 
                    _raycastHits, RigidBody.velocity.magnitude * Time.fixedDeltaTime, _rayCastDetectLayers);

                for (int i = 0; i < hits; i++)
                {
                    var hit = _raycastHits[i];
                    if (hit.collider.CompareTag(_rayCastDetectTag))
                    {
                        RigidBody.position = hit.point;
                        RigidBody.velocity = Vector3.zero;
                        break;
                    }
                }
            }
            
            UpdateTrajectory();

            RigidBody.velocity = Vector3.ClampMagnitude(RigidBody.velocity, maxSpeed);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (destroyAgainstLayers == (destroyAgainstLayers | (1 << collision.gameObject.layer)))
            {
                DestroyProjectile();
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (destroyAgainstLayers == (destroyAgainstLayers | (1 << other.gameObject.layer)))
            {
                DestroyProjectile();
            }
        }
    }
}