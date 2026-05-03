using Droids.Code.Extension;
using Droids.Component;
using Droids.Handlers;
using Droids.StateMachine;
using Droids.StateMachine.Enemy;
using System;
using UnityEngine;

namespace Droids.Behaviour
{
    public sealed class EnemyBehaviour : MonoBehaviour, IMachineContext, ITargetBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private float m_runFactor = 5f;
        [SerializeField] private float m_idleTime = 3f;
        [SerializeField] private float m_eyeHeight = 0.75f;
        [SerializeField] private float m_detectionDistance = 8f;
        [SerializeField] private float m_patrolProximity = 0.8f;
        [SerializeField] private Vector2[] m_patrolPositions;

        [Header("References")]
        [SerializeField] private CharacterControllerComponent m_controller;
        [SerializeField] private SpriteRenderer m_spriteRenderer;
        [SerializeField] private Animator m_animator;

        public float RunFactor => m_runFactor;
        public float IdleTime => m_idleTime;
        public float PatrolProximity => m_patrolProximity;
        public Vector2[] PatrolPositions => m_patrolPositions;

        public CharacterControllerComponent Controller => m_controller;
        public SpriteRenderer SpriteRenderer => m_spriteRenderer;
        public EnemyAnimatorHandler Animator { get; private set; }
        public BaseMachineState CurrentState { get; set; }

        private int m_patrolIndex;
        public int PatrolIndex
        {
            get => m_patrolIndex;
            set
            {
                int v = value;
                if (v >= m_patrolPositions.Length)
                    v = 0;

                m_patrolIndex = v;
            }
        }
        public Vector3 CurrentPatrolPosition => PatrolPositions[PatrolIndex].ToVector3();

        private Vector3 m_appliedMovement;
        public float AppliedX { get => m_appliedMovement.x; set => m_appliedMovement.x = value; }
        public float AppliedY { get => m_appliedMovement.y; set => m_appliedMovement.y = value; }

        private bool m_xFlipped;
        public bool XFlipped
        {
            get => m_xFlipped;
            set
            {
                m_xFlipped = value;
                m_spriteRenderer.flipX = !value;
                m_controller.XFlipped = value;
            }
        }

        private LayerMask m_playerLayer;
        public PlayerBehaviour Target { get; set; } = null;

        public void Move()
        {
            float x = AppliedX * Time.deltaTime;
            float y = AppliedY * Time.deltaTime;

            Vector2 vector = new(x, y);
            m_controller.Move(vector);
        }

        private Vector3 DetectionDirection
        {
            get
            {
                if (m_controller.XFlipped)
                    return Vector3.left;
                return Vector3.right;
            }
        }

        public void DetectPlayer()
        {
            Vector2 eyePosition = transform.position.ToVector2() + m_eyeHeight * Vector2.up;
            RaycastHit2D hit = Physics2D.Raycast(eyePosition, DetectionDirection, m_detectionDistance, m_playerLayer);

            if (hit.point == Vector2.zero)
                return;

            Debug.Log("Player Detected");
        }

        private void Awake()
        {
            m_playerLayer = LayerMask.GetMask("Player");
            Animator = new(m_animator);
            CurrentState = new PlayingState(this);
        }

        private void Start()
        {
            CurrentState.EnterState();
        }

        private void Update()
        {
            CurrentState.UpdateStates();
        }

        private void FixedUpdate()
        {
            CurrentState.FixedUpdateStates();
        }

        public void ReceiveDamage(int damage, bool stab)
        {
            Debug.Log($"Damage: {damage}");
        }

#if UNITY_EDITOR
        [Flags]
        public enum EnemyBehaviourGizmos
        {
            None = 0,
            PatrolPositions = 1,
            Detection = 2,
        }

        [Header("Gizmos")]
        [SerializeField] private EnemyBehaviourGizmos m_showGizmos;
        [SerializeField] private float m_patrolMarkerRadius = 0.07f;
        [SerializeField] private float m_eyeMarkerRadius = 0.07f;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            if(m_showGizmos.HasFlag(EnemyBehaviourGizmos.PatrolPositions))
            {
                foreach (Vector2 point in m_patrolPositions)
                    Gizmos.DrawSphere(point.ToVector3(), m_patrolMarkerRadius);
            }

            if (m_showGizmos.HasFlag(EnemyBehaviourGizmos.Detection))
            {
                Vector3 eyePosition = transform.position + m_eyeHeight * Vector3.up;
                Gizmos.DrawSphere(eyePosition, m_eyeMarkerRadius);
                Gizmos.DrawLine(eyePosition, eyePosition + m_detectionDistance * DetectionDirection);
            }
        }
#endif
    }
}
