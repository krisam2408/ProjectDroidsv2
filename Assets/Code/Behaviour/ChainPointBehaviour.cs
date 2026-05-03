using Droids.Code.Extension;
using Droids.Model;
using UnityEngine;

namespace Droids.Behaviour
{
    public sealed class ChainPointBehaviour : MonoBehaviour
    {
        [Header("Behaviour")]
        [SerializeField] private Vector2 m_direction = Vector2.right;

        [Header("Collision")]
        [SerializeField] private Vector2 m_origin;
        [SerializeField] private float m_radius;

        [Header("References")]
        [SerializeField] private SpriteRenderer m_sprite;
        [SerializeField] private LineRenderer m_line;

        private Vector2 m_startPosition;
        private bool m_messageSent = false;

        public float SpeedFactor { get; set; }

        public Vector2 Direction
        {
            get => m_direction;
            set => m_direction = value;
        }

        public LayerMask TargetLayers { get; set; }
        public int Damage { get; set; }

        private void Start()
        {
            m_startPosition = transform.position;
            float angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
            m_sprite.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            m_line.SetPosition(0, m_startPosition);
        }

        private void Update()
        {
            transform.Translate(SpeedFactor * Time.deltaTime * m_direction, Space.World);
            m_line.SetPosition(1, transform.position);
            
        }

        private void FixedUpdate()
        {
            Collider2D collider = Physics2D.OverlapCircle(transform.position.ToVector2() + m_origin, m_radius, TargetLayers);
            if(collider != null && !m_messageSent)
            {
                EventManager.OnChainCollision(transform.position);
                m_messageSent = true;

                if (collider.TryGetComponent(out ITargetBehaviour target))
                    target.ReceiveDamage(Damage, true);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position.ToVector2() + m_origin, m_radius);
        }
#endif
    }
}
