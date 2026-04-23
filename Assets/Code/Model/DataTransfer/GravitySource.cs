using System;
using UnityEngine;

namespace Droids.Model.DataTransfer
{
    public sealed class GravitySource
    {
        public float InitialJumpForce { get; private set; }
        public float Grounded { get; private set; }
        public float JumpForce { get; private set; }
        public float FallForce { get; private set; }
        public float MaxFallSpeed { get; private set; }

        private GravitySource() { }

        public static GravitySource Create(GravityData data)
        {
            GravitySource result = new();

            float timeToApex = data.AirTime * 0.5f;
            float jumpGravity = -2f * data.Height / Mathf.Pow(timeToApex, 2f);

            result.InitialJumpForce = 2f * data.Height / timeToApex;
            result.Grounded = GravityData.MinGravity;
            result.JumpForce = jumpGravity;
            result.FallForce = data.FallFactor * jumpGravity;
            result.MaxFallSpeed = GravityData.MaxFallSpeed;

            return result;
        }
    }

    [Serializable]
    public struct GravityData
    {
        public float AirTime;
        public float Height;
        public float FallFactor;

        public const float MinGravity = 0f;
        public const float Gravity = -9.8f;
        public const float MaxFallSpeed = -20f;
    }
}
