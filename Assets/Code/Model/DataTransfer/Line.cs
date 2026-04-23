using UnityEngine;

namespace Droids.Model.DataTransfer
{
    public struct Line
    {
        public Vector2 PointA { get; set; }
        public Vector2 PointB { get; set; }

        public Line(Vector2 pointA, Vector2 pointB)
        {
            PointA = pointA;
            PointB = pointB;
        }
    }
}
