namespace Droids.StateMachine
{
    public interface IMachineContext
    {
        public BaseMachineState CurrentState { get; set; }
    }
}
