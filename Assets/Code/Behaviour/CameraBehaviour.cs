using UnityEngine;

namespace Droids.Behaviour
{
    public sealed class CameraBehaviour : MonoBehaviour
    {
        [SerializeField] private Transform m_target;
        [SerializeField] private Vector2 m_targetOffset;
        [SerializeField] private float m_smoothing;

        private void Update()
        {
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = m_target.position + new Vector3(m_targetOffset.x, m_targetOffset.y, transform.position.z);
            transform.position = Vector3.Lerp(currentPosition, targetPosition, m_smoothing * Time.deltaTime);
        }
    }
}
