using System.Linq;
using UnityEngine;

namespace Droids.Facade.Animation
{
    public abstract class Parameter<T>
    {
        protected readonly Animator m_animator;

        private readonly int[] m_hashes;
        protected int[] Hashes => m_hashes;

        private T m_value;
        public T Value 
        {
            get => m_value;
            set 
            {
                m_value = value;
                SetAction(m_animator, value);
            }
        }

        protected Parameter(Animator animator, params string[] parameters)
        {
            m_animator = animator;

            m_hashes = parameters
                .Select(p => Animator.StringToHash(p))
                .ToArray();
        }

        protected abstract void SetAction(Animator animator, T value);
    }
}
