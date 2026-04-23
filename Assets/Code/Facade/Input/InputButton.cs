using UnityEngine;
using UnityEngine.InputSystem;

namespace Droids.Facade.Input
{
    public sealed class InputButton
    {
        private enum ButtonStateEnum
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
        private ButtonStateEnum State 
        {
            get
            {
                if (m_trigger && !m_changed && m_value)
                {
                    m_changed = true;
                    return ButtonStateEnum.Pressed;
                }

                if (m_trigger && m_changed && m_value)
                {
                    HeldTime += Time.deltaTime;
                    return ButtonStateEnum.Held;
                }

                if (!m_trigger && m_changed && !m_value)
                {
                    m_changed = false;
                    HeldTime = 0f;
                    return ButtonStateEnum.Released;
                }

                return ButtonStateEnum.Free;
            }
        }

        public bool IsPressed => State == ButtonStateEnum.Pressed;
        public bool IsHeld => State == ButtonStateEnum.Held;
        public bool IsReleased => State == ButtonStateEnum.Released;

        public void SetValues(InputAction.CallbackContext context)
        {
            m_value = context.ReadValue<float>() != 0f;
            m_trigger = context.action.triggered;
        }

        public ButtonState GetState()
        {
            return new()
            {
                IsPressed = IsPressed,
                IsHeld = IsHeld,
                IsReleased = IsReleased
            };
        }
    }

    public sealed class ButtonState
    {
        public bool IsPressed { get; set; }
        public bool IsHeld { get; set; }
        public bool IsReleased { get; set; }
    }
}