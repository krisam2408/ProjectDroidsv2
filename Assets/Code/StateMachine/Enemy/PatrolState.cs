using Droids.Behaviour;
using Droids.Code.Extension;
using System;
using UnityEngine;

namespace Droids.StateMachine.Enemy
{
    public sealed class PatrolState : BaseMachineState<EnemyBehaviour>
    {
        public PatrolState(EnemyBehaviour context) : base(context)
        {
        }

        protected override bool CheckSwitch()
        {
            float distance = Vector3.Distance(Context.transform.position, Context.CurrentPatrolPosition);
            if (distance < Context.PatrolProximity)
            {
                SwitchState(new IdleState(Context));
                return true;
            }

            return false;
        }

        public override void EnterState()
        {
            Context.PatrolIndex++;
        }

        public override void UpdateState()
        {
            if (CheckSwitch())
                return;

            Context.DetectPlayer();
            HandleMovement();
        }

        private void HandleMovement()
        {
            Vector2 cross = Context.CurrentPatrolPosition - Context.transform.position;
            cross.Normalize();

            Context.AppliedX = cross.x * Context.RunFactor;

            Context.XFlipped = cross.x < 0;
        }
    }
}
