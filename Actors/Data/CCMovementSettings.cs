using CoolTools.Utilities;
using UnityEngine;

namespace CoolTools.Actors
{
    [CreateAssetMenu(menuName = "Actor/CCMovementSettings")]
    public class CCMovementSettings : ScriptableObject
    {
        [Range(0f, 50f)]
        [SerializeField] private float _movementAccel;
        [Tooltip("How fast the character turns to face movement direction")]
        [SerializeField] private float _rotationSmoothTime;
        
        [Space(10f)]
        [SerializeField] private bool _useMoveSpeedInputCurve;
        [SerializeField] private ValueCurve _moveSpeedInputCurve;
            
        [Space(10f)]
        [SerializeField] private LayerMask _whatIsGround;
        [SerializeField] private float _terminalVelocity;
        
        [Header("Animation Parameters")]
        public string RunBlendParam = "RunBlend";
        public string StrafeBlendParam = "StrafeBlend";
        public string MotionSpeedParam = "MotionSpeed";
            
        public float TerminalVelocity => _terminalVelocity;
        public LayerMask WhatIsGround => _whatIsGround;
        public ValueCurve MoveSpeedInputCurve => _moveSpeedInputCurve;
        public float RotationSmoothTime => _rotationSmoothTime;
        public float MovementAccel => _movementAccel;
        public bool UseMoveSpeedInputCurve => _useMoveSpeedInputCurve;
        
        public void CreateHashes(out int idRunBlend, out int idStrafeBlend, out int idMotionSpeed)
        {
            idRunBlend = !string.IsNullOrEmpty(RunBlendParam)  ? Animator.StringToHash(RunBlendParam) : -1;
            idStrafeBlend = !string.IsNullOrEmpty(StrafeBlendParam)  ? Animator.StringToHash(StrafeBlendParam) : -1;
            idMotionSpeed = !string.IsNullOrEmpty(MotionSpeedParam)  ? Animator.StringToHash(MotionSpeedParam) : -1;
        }
    }
}