using Droids.Behaviour;
using Droids.Code.Extension;
using Droids.Model;
using System.Collections;
using UnityEngine;

namespace Droids.StateMachine.Player
{
    public sealed class ChainPulledState : BaseMachineState<PlayerBehaviour>
    {
        private bool m_switch = false;
        private WaitForSeconds m_wait = new(0.25f);

        public ChainPulledState(PlayerBehaviour context) : base(context) { }

        protected override bool CheckSwitch()
        {
            if(m_switch && Context.Controller.Collisions.HasFlag(CollisionChecker.Left))
            {
                SwitchState(new IdleState(Context));
                return true;
            }

            if(m_switch && Context.Controller.Collisions.HasFlag(CollisionChecker.Top))
            {
                SwitchState(new IdleState(Context));
                return true;
            }

            if(m_switch && Context.Controller.Collisions.HasFlag(CollisionChecker.Right))
            {
                SwitchState(new IdleState(Context));
                return true;
            }

            if(m_switch && Context.Controller.Collisions.HasFlag(CollisionChecker.Bottom))
            {
                SwitchState(new IdleState(Context));
                return true;
            }

            float pullDistance = Vector3.Distance(Context.transform.position, Context.ChainPullVector.ToVector3());
            if (pullDistance < Context.PullProximity)
            {
                SwitchState(new IdleState(Context));
                return true;
            }

            return false;
        }

        public override void EnterState()
        {
            Context.Skills.Locked = true;
            Context.Animator.Fall.Play();
            Context.StartCoroutine(Wait());
        }

        public override void UpdateState()
        {
            if (CheckSwitch())
                return;

            HandlePull();
        }

        public override void ExitState()
        {
            Context.Skills.Locked = false;
            Context.ChainPullVector = null;
        }

        private void HandlePull()
        {
            Vector2 cross = Context.ChainPullVector.Value - Context.transform.position.ToVector2();
            cross.Normalize();
            Vector2 translateVector = Context.PullFactor * cross;

            Context.AppliedX = translateVector.x;
            Context.AppliedY = translateVector.y;
        }

        private IEnumerator Wait()
        {
            yield return m_wait;
            m_switch = true;
        }
    }
}