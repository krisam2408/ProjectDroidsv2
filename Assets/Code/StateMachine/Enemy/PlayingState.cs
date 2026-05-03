using Droids.Behaviour;

namespace Droids.StateMachine.Enemy
{
    public sealed class PlayingState : BaseMachineState<EnemyBehaviour>
    {
        public override bool IsRoot => true;

        public PlayingState(EnemyBehaviour context) : base(context) 
        {
            InitializeSubState();
        }

        protected override void InitializeSubState()
        {
            SetSubState(new IdleState(Context));
        }

        protected override bool CheckSwitch()
        {
            return false;
        }

        public override void UpdateState()
        {
            if (CheckSwitch())
                return;
        }

        public override void FixedUpdateState()
        {
            Context.Move();
        }
    }
}
