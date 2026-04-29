using Droids.Behaviour;
using Droids.Code.Extension;
using UnityEngine;

namespace Droids.StateMachine.Player
{
    public sealed class ChainPulledState : BaseMachineState<PlayerBehaviour>
    {
        public ChainPulledState(PlayerBehaviour context) : base(context) { }

        protected override bool CheckSwitch()
        {
            if(Vector3.Distance(Context.transform.position, Context.ChainPullVector.ToVector3()) < Context.PullProximity)
            {
                SwitchState(new IdleState(Context));
                return true;
            }

            return false;
        }

        public override void EnterState()
        {
            Context.Animator.Fall.Play();
        }

        public override void UpdateState()
        {
            if (CheckSwitch())
                return;

            Context.transform.position = Vector3.Lerp(Context.transform.position, Context.ChainPullVector.ToVector3(), Context.PullFactor * Time.deltaTime);
        }

        public override void ExitState()
        {
            Context.ChainPullVector = null;
        }
    }
}
