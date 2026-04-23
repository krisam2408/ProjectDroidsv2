using UnityEngine;

namespace Droids.Component
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class DebugBoundsComponent : MonoBehaviour
    {
        [SerializeField,Range(0,5)] private float m_diameter = 0.05f;

        private void OnDrawGizmos()
        {
            Collider2D collider = GetComponent<Collider2D>();
            Bounds bounds = collider.bounds;

            Gizmos.color = Color.red;
            Vector3 topLeft = new(bounds.center.x - bounds.size.x * 0.5f, bounds.center.y + bounds.size.y * 0.5f);
            Gizmos.DrawSphere(topLeft, m_diameter);

            Gizmos.color = Color.green;
            Vector3 topRight = new(bounds.center.x + bounds.size.x * 0.5f, bounds.center.y + bounds.size.y * 0.5f);
            Gizmos.DrawSphere(topRight, m_diameter);

            Gizmos.color = Color.blue;
            Vector3 bottomRight = new(bounds.center.x + bounds.size.x * 0.5f, bounds.center.y - bounds.size.y * 0.5f);
            Gizmos.DrawSphere(bottomRight, m_diameter);

            Gizmos.color = Color.yellow;
            Vector3 bottomLeft = new(bounds.center.x - bounds.size.x * 0.5f, bounds.center.y - bounds.size.y * 0.5f);
            Gizmos.DrawSphere(bottomLeft, m_diameter);


        }
    }
}
