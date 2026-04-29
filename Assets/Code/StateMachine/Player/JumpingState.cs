using Droids.Behaviour;
using Droids.Model;
using System;

namespace Droids.StateMachine.Player
{
    public sealed class JumpingState : BaseMachineState<PlayerBehaviour>
    {
        public JumpingState(PlayerBehaviour context) : base(context) { }

        protected override bool CheckSwitch()
        {
            if(Context.Input.Jump.IsReleased)
            {
                SwitchState(new FallingState(Context));
                return true;
            }

            if (Context.Controller.Collisions.HasFlag(CollisionChecker.Top))
            {
                SwitchState(new FallingState(Context, true));
                return true;
            }

            if(Context.AppliedY < 0f)
            {
                SwitchState(new FallingState(Context));
                return true;
            }

            return false;
        }

        public override void EnterState()
        {
            HandleJump(Context.Gravities.InitialJumpForce);
            Context.Animator.Jump.Play();
        }

        public override void FixedUpdateState()
        {
            HandleJump(Context.Gravities.FallForce);
        }

        public override void ExitState()
        {

        }

        private void HandleJump(float force)
        {
            float previous = Context.AppliedY;
            float next = previous + force;
            float avg = (previous + next) * 0.5f;
            Context.AppliedY = avg;
        }
    }
}
