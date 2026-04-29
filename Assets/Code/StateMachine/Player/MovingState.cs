using Droids.Behaviour;
using Droids.Model;
using System;
using UnityEngine;

namespace Droids.StateMachine.Player
{
    public sealed class MovingState : BaseMachineState<PlayerBehaviour>
    {
        public MovingState(PlayerBehaviour context) : base(context)
        {
            InitializeSubState();
        }

        protected override void InitializeSubState()
        {
            if (!Context.Controller.Collisions.HasFlag(CollisionChecker.Bottom))
            {
                SetSubState(new FallingState(Context));
                return;
            }

            SetSubState(new GroundedState(Context));
        }

        protected override bool CheckSwitch()
        {
            if (!Context.Input.Move.IsHorizontalActive)
            {
                SwitchState(new IdleState(Context));
                return true;
            }

            return false;
        }

        public override void EnterState()
        {
        }

        public override void UpdateState()
        {
            if (CheckSwitch())
                return;

            Context.AppliedX = Context.Input.Move.X;
            HandleSpriteDirection();
        }

        public override void ExitState()
        {
            
        }

        private void HandleSpriteDirection()
        {
            if (!Context.Controller.CanMove)
                return;

            float sign = Mathf.Sign(Context.Input.Move.X);
            Context.XFlipped = sign < 0f ? true : false;
        }
    }
}
