using UnityEngine;
using UnityEngine.InputSystem;

namespace Droids.Facade.Input
{
    public sealed class InputAxis
    {
        public enum Direction
        {
            None,
            Up,
            Right,
            Down,
            Left
        }

        private float m_lastX;
        private float m_x;
        public float X
        {
            get => m_x;
            private set
            {
                if (value != 0f)
                    m_lastX = value;
                m_x = value;
            }
        }

        private float m_lastY;
        private float m_y;
        public float Y
        {
            get => m_y;
            private set
            {
                if (value != 0f)
                    m_lastY = value;
                m_y = value;
            }
        }

        public bool IsHorizontalActive => X != 0f;
        public bool IsVerticalActive => Y != 0f;
        public bool IsActive => IsHorizontalActive || IsVerticalActive;

        public Vector2 Axis => new(X, Y);

        public void SetValues(InputAction.CallbackContext context)
        {
            Vector2 axis = context.ReadValue<Vector2>();
            X = axis.x;
            Y = axis.y;
        }

        public Direction DirectionOnRelease
        {
            get
            {
                if (m_lastX == 0f && m_lastY == 0f)
                    return Direction.None;

                float absX = Mathf.Abs(m_lastX);
                float absY = Mathf.Abs(m_lastY);

                if (absX == absY)
                    return Direction.None;

                if (absX > absY)
                {
                    if (m_lastX > 0)
                        return Direction.Right;
                    return Direction.Left;
                }

                if (m_lastY > 0)
                    return Direction.Up;

                return Direction.Down;
            }
        }
    }
}
