using Droids.Facade.Input;

namespace Droids.Handlers
{
    public sealed class PlayerInputHandler
    {
        public InputAxis Move { get; private set; } = new();
        public InputButton Slash { get; private set; } = new();
        public InputButton Stab { get; private set; } = new();
        public InputButton Jump { get; private set; } = new();
        public InputButton Chain { get; private set; } = new();
        public InputButton ContextMenu { get; private set; } = new();

        public AttacksState GetState()
        {
            return new()
            {
                Slash = Slash.GetState(),
                Stab = Stab.GetState(),
                Chain = Chain.GetState()
            };
        }
    }

    public class AttacksState
    {
        public ButtonState Slash { get; set; }
        public ButtonState Stab { get; set; }
        public ButtonState Chain { get; set; }
    }
}
