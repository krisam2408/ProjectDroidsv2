using Droids.Behaviour;
using Droids.Model.DataTransfer;
using Droids.Model.Skills.Player;
using System;
using UnityEngine;

namespace Droids.StateMachine.Player
{
    public sealed class PlayingState : BaseMachineState<PlayerBehaviour>
    {
        public override bool IsRoot => true;

        public PlayingState(PlayerBehaviour context) : base(context)
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

        public override void EnterState()
        {
            Context.Gravities = GravitySource.Create(Context.JumpParams);
        }

        public override void UpdateState()
        {
            if (CheckSwitch())
                return;

            HandleSkills();
        }

        public override void FixedUpdateState()
        {
            Context.Move();
        }

        private void HandleSkills()
        {
            BasePlayerSkill skill = Context.Skills.CheckSkills();

            if(skill != null)
            {
                Context.StartCoroutine(skill.Execute());
            }
        }
    }
}
