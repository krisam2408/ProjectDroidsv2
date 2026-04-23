using UnityEngine;
using UnityEngine.InputSystem;

namespace Droids.Facade.Input
{
    public sealed class InputButton
    {
        public enum ButtonState
        {
            Free,
            Pressed,
            Held,
            Released
        }

        private bool m_value;
        private bool m_trigger;
        private bool m_changed;

        public float HeldTime { get; private set; }
        public ButtonState State 
        {
            get
            {
                if (m_trigger && !m_changed && m_value)
                {
                    m_changed = true;
                    return ButtonState.Pressed;
                }

                if (m_trigger && m_changed && m_value)
                {
                    HeldTime += Time.deltaTime;
                    return ButtonState.Held;
                }

                if (!m_trigger && m_changed && !m_value)
                {
                    m_changed = false;
                    HeldTime = 0f;
                    return ButtonState.Released;
                }

                return ButtonState.Free;
            }
        }

        public bool IsPressed => State == ButtonState.Pressed;
        public bool IsHeld => State == ButtonState.Held;
        public bool IsReleased => State == ButtonState.Released;

        public void SetValues(InputAction.CallbackContext context)
        {
            m_value = context.ReadValue<float>() != 0f;
            m_trigger = context.action.triggered;
        }
    }
}