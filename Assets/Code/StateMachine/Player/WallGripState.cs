using Droids.Behaviour;
using Droids.Model;

namespace Droids.StateMachine.Player
{
    public sealed class WallGripState : BaseMachineState<PlayerBehaviour>
    {
        public WallGripState(PlayerBehaviour context) : base(context) { }

        protected override bool CheckSwitch()
        {
            if(!Context.Input.Jump.IsHeld)
            {
                SwitchState(new FallingState(Context));
                return true;
            }

            if(Context.Controller.Collisions.HasFlag(CollisionChecker.Bottom))
            {
                SwitchState(new GroundedState(Context));
                return true;
            }

            return false;
        }

        public override void EnterState()
        {
            Context.WallGripped = true;
            Context.Animator.WallGrip.Value = true;
            Context.Controller.CanMove = false;
            Context.AppliedX = 0f;
            Context.AppliedY = 0f;
        }

        public override void ExitState()
        {
            Context.WallGripped = false;
            Context.Animator.WallGrip.Value = false;
            Context.Controller.CanMove = true;
        }
    }
}
