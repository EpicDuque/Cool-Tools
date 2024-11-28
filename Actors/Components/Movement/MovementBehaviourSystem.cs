using System.Collections.Generic;
using CoolTools.Utilities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace CoolTools.Actors
{
    public class MovementBehaviourSystem : Singleton<MovementBehaviourSystem>
    {
        private static List<MovementBehaviour> Instances = new ();

        public static bool Instanced = false;

        [SerializeField] private LayerMask _whatIsGround;
        [SerializeField] private float _castRadius = 0.4f;
        [SerializeField] private float _castDistance = 0.2f;
        
        [Space(10f)] 
        [SerializeField] private int _workBatchCount = 2;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Reload()
        {
            Instance = null;
            Instances.Clear();
            Instanced = false;
        }
        
        private new void Awake()
        {
            base.Awake();

            if (!Instanced)
            {
                DontDestroyOnLoad(gameObject);
                Instanced = true;
            }
        }

        public static void RegisterInstance(MovementBehaviour instance)
        {
            Instances.Add(instance);
        }
        
        public static void UnregisterInstance(MovementBehaviour instance)
        {
            Instances.Remove(instance);
        }

        private void Update()
        {
            return;
            var infos = new NativeArray<MovementInfo>(Instances.Count, Allocator.TempJob);
            var transforms = new TransformAccessArray(Instances.Count, 8);
            
            for (int i = 0; i < Instances.Count; i++)
            {
                var instance = Instances[i];
                var info = new MovementInfo
                {
                    Index = i,
                    Position = instance.transform.position,
                    Velocity = float3.zero,
                    AgentVelocity = instance.HasNavMeshAgent ? instance.NavMeshAgent.velocity : 0,
                    CCVelocity = instance.HasCharacterController ? instance.CharacterController.velocity : 0,
                    MaxMoveSpeed = instance.MaxMovementSpeed.Value,
                    Speed = instance.Speed,
                    MaxLerp = instance.MaxMovementSpeed.Value,
                    Accel = instance.Acceleration,
                    LastPosition = instance.transform.position,
                    HasNavMeshAgent = instance.HasNavMeshAgent,
                    HasCC = instance.HasCharacterController,
                    DeltaTime = Time.deltaTime,
                    CanRotate = instance.CanRotate,
                    AgentCanRotate = instance.HasNavMeshAgent && instance.NavMeshAgent.updateRotation,
                    LookInput = instance.LookInput,
                    RotationSmoothTime = instance.Settings.RotationSmoothTime,
                };
                infos[i] = info;
                transforms.Add(instance.transform);
            }

            var job = new UpdateSpeedJob
            {
                Infos = infos
            };
            
            var handle = job.Schedule(transforms);
            handle.Complete();

            for (int i = 0; i < infos.Length; i++)
            {
                var info = infos[i];
                var instance = Instances[i];
                instance.Speed = info.Speed;
                instance.Velocity = info.Velocity;
            }
            
            infos.Dispose();
            transforms.Dispose();
        }

        private void FixedUpdate()
        {
            var commands = new NativeArray<SpherecastCommand>(Instances.Count, Allocator.TempJob);
            var results = new NativeArray<RaycastHit>(Instances.Count, Allocator.TempJob);

            for (int i = 0; i < Instances.Count; i++)
            {
                var instance = Instances[i];
                var position = instance.transform.position + Vector3.up * 0.15f;

                var parameters = new QueryParameters(_whatIsGround);
                commands[i] = new SpherecastCommand(position, _castRadius, Vector3.down, parameters, _castDistance);
            }
            
            var handle = SpherecastCommand.ScheduleBatch(commands, results, 2);
            handle.Complete();

            for (int i = 0; i < results.Length; i++)
            {
                var result = results[i];
                var instance = Instances[i];
                instance.IsGrounded = false;

                if (result.colliderInstanceID == 0)
                {
                    continue;
                }
                
                instance.IsGrounded = true;
            }

            commands.Dispose();
            results.Dispose();
        }

        [BurstCompile]
        private struct MovementInfo
        {
            public int Index;
            public float3 Position;
            public float3 Velocity;
            public float3 AgentVelocity;
            public float3 CCVelocity;
            public float MaxMoveSpeed;
            public float Speed;
            public float MaxLerp;
            public float Accel;
            public float3 LastPosition;
            public bool HasNavMeshAgent;
            public bool HasCC;
            public float DeltaTime;
            public bool CanRotate;
            public bool AgentCanRotate;
            public float3 LookInput;
            public float RotationSmoothTime;
            
            public void UpdateSpeed()
            {
                if (HasNavMeshAgent)
                {
                    Velocity = AgentVelocity;
                } else if (HasCC)
                {
                    Velocity = CCVelocity;
                }
                else
                {
                    Velocity = (Position - LastPosition) / DeltaTime;
                }
            
                LastPosition = Position;
                var currentSpeed = math.length(Velocity);

                const float speedOffset = 0.1f;
            
                // accelerate or decelerate to target speed
                if (currentSpeed < MaxMoveSpeed - speedOffset || currentSpeed > MaxMoveSpeed + speedOffset)
                {
                    Speed = math.lerp(currentSpeed, MaxLerp, DeltaTime * Accel);

                    Speed = math.round(Speed * 1000) / 1000f;
                }
                else
                {
                    Speed = MaxMoveSpeed;
                }
            }
            
            public void RotateStep(TransformAccess transform)
            {
                if (DeltaTime <= 0f) return;
                if (!CanRotate) return;

                if (HasNavMeshAgent && AgentCanRotate) return;

                var target = quaternion.LookRotation(LookInput, Vector3.up);
                
                transform.rotation = math.slerp(transform.rotation, target, DeltaTime * RotationSmoothTime * 30f);
            }
        }
        
        [BurstCompile]
        private struct UpdateSpeedJob : IJobParallelForTransform
        {
            public NativeArray<MovementInfo> Infos;

            public void Execute(int index, TransformAccess transform)
            {
                var info = Infos[index];
                info.UpdateSpeed();
                info.RotateStep(transform);
                Infos[index] = info;
            }
        }
    }
}