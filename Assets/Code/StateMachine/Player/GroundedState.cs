using Droids.Behaviour;
using Droids.Model;
using UnityEngine;
using System;

namespace Droids.StateMachine.Player
{
    public sealed class GroundedState : BaseMachineState<PlayerBehaviour>
    {
        private bool DownPassThrough
        {
            get
            {
                bool passThrough = Context.Controller.PassThrough.HasFlag(CollisionChecker.Bottom);
                bool downInput = Context.Input.Move.Y < 0f;

                return passThrough && downInput;
            }
        }

        public GroundedState(PlayerBehaviour context) : base(context) { }

        protected override bool CheckSwitch()
        {
            if(!Context.Controller.Collisions.HasFlag(CollisionChecker.Bottom))
            {
                SwitchState(new FallingState(Context));
                return true;
            }

            if(!DownPassThrough && Context.Input.Jump.IsPressed)
            {
                SwitchState(new JumpingState(Context));
                return true;
            }

            return false;
        }

        public override void EnterState()
        {
            Context.AppliedY = 0f;
            Context.Animator.Grounded.Value = true;
            Context.Animator.Hung.Value = false;
            Context.Controller.CanMove = true;
        }

        public override void UpdateState()
        {
            if (CheckSwitch())
                return;

            HandleBottomPassThrough();
        }

        public override void ExitState()
        {
            Context.Animator.Grounded.Value = false;
        }

        private void HandleBottomPassThrough()
        {
            bool jumpInput = Context.Input.Jump.IsPressed;

            if (DownPassThrough && jumpInput)
            {
                Context.Controller.DisableCollisionToggle(CollisionChecker.Bottom);
            }
        }
    }
}
