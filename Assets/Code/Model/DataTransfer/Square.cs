using UnityEngine;

namespace Droids.Model.DataTransfer
{
    public struct Square
    {
        public Vector2 Origin { get; private set; }
        public Vector2 HalfExtents { get; private set; }

        public Vector2 TopLeft { get; private set; }
        public Vector2 TopRight { get; private set; }
        public Vector2 BottomLeft { get; private set; }
        public Vector2 BottomRight { get; private set; }

        public Square(Vector2 origin, Vector2 halfExtents)
        {
            Origin = origin;
            HalfExtents = halfExtents;
            TopLeft = origin + new Vector2(-halfExtents.x, halfExtents.y);
            TopRight = origin + new Vector2(halfExtents.x, halfExtents.y);
            BottomRight = origin + new Vector2(halfExtents.x, -halfExtents.y);
            BottomLeft = origin + new Vector2(-halfExtents.x, -halfExtents.y);
        }

        public Line[] GetLines()
        {
            return new Line[]
            {
                new(TopLeft, TopRight),
                new(TopLeft, BottomLeft),
                new(TopRight, BottomRight),
                new(BottomLeft, BottomRight)
            };
        }

        public void DrawGizmo()
        {
            Gizmos.DrawLine(TopLeft, TopRight);
            Gizmos.DrawLine(TopLeft, BottomLeft);
            Gizmos.DrawLine(TopRight, BottomRight);
            Gizmos.DrawLine(BottomLeft, BottomRight);
        }
    }
}
