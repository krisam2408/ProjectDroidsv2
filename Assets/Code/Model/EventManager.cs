using System;
using UnityEngine;

namespace Droids.Model
{
    public static class EventManager
    {
        public static event Action<Vector2> ChainCollision;
        public static void OnChainCollision(Vector2 vector) => ChainCollision?.Invoke(vector);
    }
}
