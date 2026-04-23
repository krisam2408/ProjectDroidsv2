using Droids.Component;
using Droids.Handlers;
using Droids.Model;
using Droids.Model.DataTransfer;
using Droids.StateMachine;
using Droids.StateMachine.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Droids.Behaviour
{
    public class PlayerBehaviour : MonoBehaviour, IMachineContext
    {
        [Header("Parameters")]
        [SerializeField] private float m_runFactor = 7f;
        [SerializeField] private float m_pullFactor = 12f;
        [SerializeField] private float m_pullProximity = 1f;
        [SerializeField] private GravityData m_jumpParams;

        [Header("Chain Firing Spot")]
        [SerializeField] private float m_topAltitude;
        [SerializeField] private float m_bottomAltitude;
        [SerializeField] private float m_radius;

        [Header("Components")]
        [SerializeField] private CharacterControllerComponent m_controller;
        [SerializeField] private SpriteRenderer m_spriteRenderer;
        [SerializeField] private Animator m_spriteAnimator;
        [SerializeField] private PlayerSkillsComponent m_skills;

        public float PullFactor => m_pullFactor;
        public float PullProximity => m_pullProximity;
        public GravityData JumpParams => m_jumpParams;

        public CharacterControllerComponent Controller => m_controller;
        public SpriteRenderer SpriteRenderer => m_spriteRenderer;
        public PlayerSkillsComponent Skills => m_skills;
        public PlayerAnimatorHandler Animator { get; private set; }
        public PlayerInputHandler Input { get; private set; } = new();
        public BaseMachineState CurrentState { get; set; }
        public GravitySource Gravities { get; set; }

        private Vector3 m_appliedMovement;
        public float AppliedX { get => m_appliedMovement.x; set => m_appliedMovement.x = value; }
        public float AppliedY { get => m_appliedMovement.y; set => m_appliedMovement.y = value; }

        public Vector2? ChainPullVector { get; set; } = null;

        private bool m_xFlipped;
        public bool XFlipped
        {
            get => m_xFlipped;
            set
            {
                m_xFlipped = value;
                m_spriteRenderer.flipX = value;
                m_controller.XFlipped = value;
            }
        }

        public bool WallGripped { get; set; }

        public void Move()
        {
            float x = m_runFactor * AppliedX * Time.deltaTime;
            float y = AppliedY * Time.deltaTime;
            
            Vector2 vector = new(x, y);
            m_controller.Move(vector);
        }

        public Vector3[] SetChainFiringSpot(float x, float y)
        {
            Vector3[] result;
            Vector3 input = new(x, y);
            if(input == Vector3.zero)
            {
                if (XFlipped)
                {
                    result = new Vector3[]
                    {
                        Vector3.left,
                        transform.position + m_topAltitude * Vector3.up + m_radius * Vector3.left
                    };
                    return result;
                }

                result = new Vector3[]
                {
                    Vector3.right,
                    transform.position + m_topAltitude * Vector3.up + m_radius * Vector3.right
                };
                return result;
            }

            Vector3 startPosition;
            Vector3 addPosition;
            Vector3 normal = input.normalized;

            if (y < 0)
            {
                startPosition = transform.position + m_bottomAltitude * Vector3.up;
                addPosition = m_radius * input;

                result = new Vector3[]
                {
                    normal,
                    startPosition + addPosition
                };

                return result;
            }

            startPosition = transform.position + m_topAltitude * Vector3.up;
            addPosition = m_radius * input;

            result = new Vector3[]
            {
                    normal,
                    startPosition + addPosition
            };

            return result;
        }

        private void Awake()
        {
            Animator = new(m_spriteAnimator);
            CurrentState = new PlayingState(this);
        }

        private void OnEnable()
        {
            EventManager.ChainCollision += GetChainPullVector;
        }

        private void Start()
        {
            CurrentState.EnterState();
            Skills.Initialize(this);
        }

        private void Update()
        {
            CurrentState.UpdateStates();

            if (Input.ContextMenu.IsReleased)
                Debug.Log(CurrentState.LogStates());
        }

        private void FixedUpdate()
        {
            CurrentState.FixedUpdateStates();
        }

        private void OnDisable()
        {
            EventManager.ChainCollision -= GetChainPullVector;
        }

        public void OnMove(InputAction.CallbackContext context) => Input.Move.SetValues(context);
        public void OnSlash(InputAction.CallbackContext context) => Input.Slash.SetValues(context);
        public void OnStab(InputAction.CallbackContext context) => Input.Stab.SetValues(context);
        public void OnJump(InputAction.CallbackContext context) => Input.Jump.SetValues(context);
        public void OnChain(InputAction.CallbackContext context) => Input.Chain.SetValues(context);
        public void OnContextMenu(InputAction.CallbackContext context) => Input.ContextMenu.SetValues(context);

        private void GetChainPullVector(Vector2 vector) => ChainPullVector = vector + m_topAltitude * Vector2.down;

#if UNITY_EDITOR
        [Header("Gizmos")]
        [SerializeField] private bool m_showGizmos;
        [SerializeField, Range(-1, 1)] private float m_virtualX;
        [SerializeField, Range(-1, 1)] private float m_virtualY;
        [SerializeField, Range(0, 3)] private float m_circleRadius;

        private void OnDrawGizmosSelected()
        {
            if (!m_showGizmos)
                return;

            Gizmos.color = Color.green;
            Vector3 topPosition = transform.position + m_topAltitude * Vector3.up;
            Vector3 bottomPosition = transform.position + m_bottomAltitude * Vector3.up;

            Gizmos.DrawSphere(topPosition, m_circleRadius);
            Gizmos.DrawSphere(bottomPosition, m_circleRadius);
            Gizmos.DrawWireSphere(topPosition, m_radius);
            Gizmos.DrawWireSphere(bottomPosition, m_radius);

            Gizmos.color = Color.blue;
            Vector3 firingPosition = SetChainFiringSpot(m_virtualX, m_virtualY)[1];
            Gizmos.DrawSphere(firingPosition, m_circleRadius);
        }
#endif
    }
}
