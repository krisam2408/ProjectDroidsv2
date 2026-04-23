using System.Text;
using UnityEditor;

namespace Droids.StateMachine
{
    public abstract class BaseMachineState
    {
        protected BaseMachineState CurrentSuperState { get; set; }
        protected BaseMachineState CurrentSubState { get; set; }

        public virtual bool IsRoot => false;

        public void UpdateStates()
        {
            UpdateState();
            if (CurrentSubState != null)
                CurrentSubState.UpdateStates();
        }

        public void FixedUpdateStates()
        {
            FixedUpdateState();
            if (CurrentSubState != null)
                CurrentSubState.FixedUpdateStates();
        }

        public void LateUpdateStates()
        {
            LateUpdateState();
            if (CurrentSubState != null)
                CurrentSubState.LateUpdateStates();
        }

        public void SetSubState(BaseMachineState newSubState, bool enterState = true)
        {
            CurrentSubState = newSubState;
            newSubState.CurrentSuperState = this;
            if (enterState)
                CurrentSubState.EnterState();
        }

        protected virtual void SwitchState(BaseMachineState state)
        {
            if (state != null && state != this)
            {
                ExitState();
                state.EnterState();

                if (CurrentSuperState != null)
                    CurrentSuperState.SetSubState(state, false);
            }
        }

        public virtual void EnterState() { }

        public virtual void UpdateState()
        {
            if (CheckSwitch())
                return;
        }

        public virtual void FixedUpdateState() { }
        public virtual void LateUpdateState() { }
        public virtual void ExitState() { }

        protected abstract bool CheckSwitch();

        protected virtual void InitializeSubState() { }

        public string LogStates()
        {
            StringBuilder sb = new();
            sb.AppendLine(GetType().Name);

            BaseMachineState sub = CurrentSubState;

            while (sub != null)
            {
                sb.AppendLine(sub.GetType().Name);
                sub = sub.CurrentSubState;
            }

            return sb.ToString();
        }
    }

    public abstract class BaseMachineState<T> : BaseMachineState where T : IMachineContext
    {
        private readonly T m_context;
        protected T Context => m_context;

        protected BaseMachineState(T context)
        {
            m_context = context;
        }

        protected override void SwitchState(BaseMachineState state)
        {
            if (state != null && state != this)
            {
                ExitState();
                state.EnterState();

                if (IsRoot)
                {
                    Context.CurrentState = state;
                    return;
                }

                if (CurrentSuperState != null)
                    CurrentSuperState.SetSubState(state, false);
            }
        }
    }
}
