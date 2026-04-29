using Droids.Behaviour;
using Droids.Model;

namespace Droids.StateMachine.Player
{
    public sealed class HungState : BaseMachineState<PlayerBehaviour>
    {
        public HungState(PlayerBehaviour context) : base(context) { }

        protected override bool CheckSwitch()
        {
            if(Context.Input.Move.Y > 0f)
            {
                SwitchState(new ClimbState(Context));
                return true;
            }

            if (!Context.Input.Jump.IsHeld)
            {
                SwitchState(new FallingState(Context));
                return true;
            }

            if (Context.Controller.Collisions.HasFlag(CollisionChecker.Bottom))
            {
                SwitchState(new GroundedState(Context));
                return true;
            }

            return false;
        }

        public override void EnterState()
        {
            Context.Animator.Hung.Play();
            Context.Controller.CanMove = false;
            Context.AppliedX = 0f;
            Context.AppliedY = 0f;
        }

        public override void FixedUpdateState()
        {
            Context.Controller.FixHungPosition();
        }
    }
}
