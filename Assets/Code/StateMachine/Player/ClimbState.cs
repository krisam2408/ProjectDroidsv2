using Droids.Behaviour;
using System.Collections;
using UnityEngine;

namespace Droids.StateMachine.Player
{
    public sealed class ClimbState : BaseMachineState<PlayerBehaviour>
    {
        private const float m_time = 1f;
        private bool m_switch = false;
        private WaitForSeconds m_wait = new(m_time);
        private Bounds? m_bounds;

        public ClimbState(PlayerBehaviour context) : base(context) { }

        protected override bool CheckSwitch()
        {
            if(m_switch)
            {
                SwitchState(new GroundedState(Context));
                return true;
            }

            return false;
        }

        public override void EnterState()
        {
            Context.Skills.Locked = true;
            Context.Animator.Climb.Value = true;
            m_bounds = Context.Controller.StartClimb();
            Context.StartCoroutine(Wait());
        }

        public override void UpdateState()
        {
            if (CheckSwitch())
                return;

            Context.Controller.Climb(m_bounds, m_time);
        }

        public override void ExitState()
        {
            m_bounds = null;
            Context.Skills.Locked = false;
            Context.Animator.Hung.Value = false;
            Context.Controller.CanMove = true;
        }

        private IEnumerator Wait()
        {
            yield return m_wait;
            m_switch = true;
        }
    }
}
