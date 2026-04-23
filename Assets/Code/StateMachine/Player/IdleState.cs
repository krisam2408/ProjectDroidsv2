using Droids.Behaviour;
using Droids.Model;

namespace Droids.StateMachine.Player
{
    public sealed class IdleState : BaseMachineState<PlayerBehaviour>
    {
        public IdleState(PlayerBehaviour context) : base(context)
        {
            InitializeSubState();
        }

        protected override void InitializeSubState()
        {
            if(!Context.Controller.Collisions.HasFlag(CollisionChecker.Bottom))
            {
                SetSubState(new FallingState(Context));
                return;
            }

            SetSubState(new GroundedState(Context));
        }

        protected override bool CheckSwitch()
        {
            if (Context.Controller.CanMove && Context.Input.Move.IsHorizontalActive)
            {
                SwitchState(new MovingState(Context));
                return true;
            }

            if(Context.ChainPullVector != null)
            {
                SwitchState(new ChainPulledState(Context));
                return true;
            }

            return false;
        }

        public override void EnterState()
        {
            Context.AppliedX = 0f;
        }
    }
}
