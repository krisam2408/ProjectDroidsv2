using Droids.Behaviour;
using Droids.Model;
using Droids.Model.DataTransfer;
using System;
using UnityEngine;

namespace Droids.StateMachine.Player
{
    public sealed class FallingState : BaseMachineState<PlayerBehaviour>
    {
        private bool m_interrupt;

        private bool WallGripConditions => Context.Controller.CanWallGrip;
        private bool HangConditions => Context.Controller.CanHangLeft || Context.Controller.CanHangRight;

        public FallingState(PlayerBehaviour context, bool jumpInterrupt = false) : base(context) 
        {
            m_interrupt = jumpInterrupt;
        }

        protected override bool CheckSwitch()
        {
            if(Context.Controller.Collisions.HasFlag(CollisionChecker.Bottom))
            {
                SwitchState(new GroundedState(Context));
                return true;
            }

            if(Context.Input.Jump.IsHeld && WallGripConditions)
            {
                SwitchState(new WallGripState(Context));
                return true;
            }

            if(Context.Input.Jump.IsHeld && HangConditions)
            {
                SwitchState(new HungState(Context));
                return true;
            }

            return false;
        }

        public override void EnterState()
        {
            Context.Animator.Fall.Play();
            Context.Controller.CanMove = true;

            if (m_interrupt)
                Context.AppliedY = 0f;
        }

        public override void FixedUpdateState()
        {
            HandleFall();
        }

        private void HandleFall()
        {
            float previous = Context.AppliedY;
            float next = previous + Context.Gravities.FallForce;
            float avg = (previous + next) * 0.5f;

            if (avg < GravityData.MaxFallSpeed)
                avg = GravityData.MaxFallSpeed;

            Context.AppliedY = avg;
        }
    }
}
