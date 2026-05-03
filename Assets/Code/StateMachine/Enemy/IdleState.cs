using Droids.Behaviour;
using System.Collections;
using UnityEngine;

namespace Droids.StateMachine.Enemy
{
    public sealed class IdleState : BaseMachineState<EnemyBehaviour>
    {
        private bool m_switch = false;

        public IdleState(EnemyBehaviour context) : base(context)
        {

        }

        protected override bool CheckSwitch()
        {
            if (m_switch)
            {
                SwitchState(new PatrolState(Context));
                return true;
            }

            return false;
        }

        public override void EnterState()
        {
            Context.StartCoroutine(Wait());
            Context.AppliedX = 0f;
        }

        public override void UpdateState()
        {
            if (CheckSwitch())
                return;

            Context.DetectPlayer();
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(Context.IdleTime);
            m_switch = true;
        }
    }
}
